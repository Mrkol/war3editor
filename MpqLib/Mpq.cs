using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using CompressionMode = Ionic.Zlib.CompressionMode;


namespace MpqLib
{
    public class Mpq
    {
        public const uint PosHashType = 0;
        public const uint AHashType = 1;
        public const uint BHashType = 2;
        public const uint FileHashType = 3;
        public static uint[] CryptTable;

        static Mpq()
        {
            CryptTable = new uint[0x500];

            uint seed = 0x00100001;

            for (uint i = 0; i < 0x100; i++)
            {
                uint j = i;
                for (int k = 0; k < 5; k++)
                {
                    seed = (seed*125 + 3)%0x2aaaab;
                    uint tmp1 = (seed & 0xffff) << 0x10;

                    seed = (seed*125 + 3)%0x2aaaab;
                    uint tmp2 = seed & 0xffff;

                    CryptTable[j] = tmp1 | tmp2;
                    j += 0x100;
                }
            }
        }

        public class FileDescriptor
        {
            public Stream InputStream { get; }

            public uint HeaderOffset { get; }

            private Header? _header;
            public Header Header {
                get
                {
                    if (_header != null) return _header.Value;

                    InputStream.Seek(HeaderOffset, SeekOrigin.Begin);
                    _header = MpqLib.Mpq.Header.Read(InputStream);

                    return _header.Value;
                }
            }

            private HashTableEntry[] _hashTable;
            public HashTableEntry[] HashTable
            {
                get
                {
                    if (_hashTable != null) return _hashTable;

                    InputStream.Seek(HeaderOffset + Header.HashTableOffset, SeekOrigin.Begin);
                    _hashTable = ReadHashTable(this);

                    return _hashTable;
                }
            }

            private BlockTableEntry[] _blockTable;
            public BlockTableEntry[] BlockTable {
                get
                {
                    if (_blockTable != null) return _blockTable;

                    InputStream.Seek(HeaderOffset + Header.BlockTableOffset, SeekOrigin.Begin);
                    _blockTable = ReadBlockTable(this);

                    return _blockTable;
                }
            }

            public FileDescriptor(Stream s)
            {
                InputStream = s;

                byte[] marker = new byte[4];
                InputStream.Read(marker, 0, 4);
                while (marker[0] != 0x4d || marker[1] != 0x50
                    || marker[2] != 0x51 || marker[3] != 0x1a) //MPQ marker
                {
                    HeaderOffset += 0x200;
                    InputStream.Seek(HeaderOffset, SeekOrigin.Begin);
                    InputStream.Read(marker, 0, 4);
                }
            }
        }

        public struct Header
        {
            public uint HeaderSize;
            public uint ArchiveSize;
            public ushort FormatVersion;
            public ushort SectorSizeShift;
            public uint HashTableOffset;
            public uint BlockTableOffset;
            public uint HashTableEntries;
            public uint BlockTableEntries;

            public static Header Read(Stream mpq)
            {
                mpq.Seek(4, SeekOrigin.Current);

                Header h = new Header();
                byte[] buffer = new byte[4];

                mpq.Read(buffer, 0, 4);
                h.HeaderSize = BitConverter.ToUInt32(buffer, 0);
                mpq.Read(buffer, 0, 4);
                h.ArchiveSize = BitConverter.ToUInt32(buffer, 0);
                mpq.Read(buffer, 0, 2);
                h.FormatVersion = BitConverter.ToUInt16(buffer, 0);
                mpq.Read(buffer, 0, 2);
                h.SectorSizeShift = BitConverter.ToUInt16(buffer, 0);
                mpq.Read(buffer, 0, 4);
                h.HashTableOffset = BitConverter.ToUInt32(buffer, 0);
                mpq.Read(buffer, 0, 4);
                h.BlockTableOffset = BitConverter.ToUInt32(buffer, 0);
                mpq.Read(buffer, 0, 4);
                h.HashTableEntries = BitConverter.ToUInt32(buffer, 0);
                mpq.Read(buffer, 0, 4);
                h.BlockTableEntries = BitConverter.ToUInt32(buffer, 0);

                return h;
            }
        }

        public struct HashTableEntry
        {
            public uint HashA;
            public uint HashB;
            public ushort Lang;
            public ushort Platform;
            public uint Index;

            public const uint EmptyEntryIndex = 0xffffffff;
            public const uint DeletedEntryIndex = 0xfffffffe;
            public const uint Size = 16;

            public bool IsEmpty => Index == EmptyEntryIndex;
            public bool IsDeleted => Index == DeletedEntryIndex;

            public HashTableEntry(byte[] data, int offset)
            {
                HashA = BitConverter.ToUInt32(data, offset);
                HashB = BitConverter.ToUInt32(data, offset + 0x4);
                Lang = BitConverter.ToUInt16(data, offset + 0x8);
                Platform = BitConverter.ToUInt16(data, offset + 0xa);
                Index = BitConverter.ToUInt32(data, offset + 0xc);
            }
        }

        public struct BlockTableEntry
        {
            public uint BlockOffset;
            public uint BlockSize;
            public uint FileSize;
            public uint Flags;

            public bool Implode => (Flags & 0x00000100) != 0;
            public bool Compress => (Flags & 0x00000200) != 0;
            public bool Encrypted => (Flags & 0x00010000) != 0;
            public bool AdjustKey => (Flags & 0x00020000) != 0;
            public bool Patch => (Flags & 0x01000000) != 0;
            public bool Single => (Flags & 0x02000000) != 0;
            public bool Delete => (Flags & 0x04000000) != 0;
            public bool Exists => (Flags & 0x80000000) != 0;

            public const uint Size = 16;

            public BlockTableEntry(byte[] data, int offset)
            {
                BlockOffset = BitConverter.ToUInt32(data, offset);
                BlockSize = BitConverter.ToUInt32(data, offset + 0x4);
                FileSize = BitConverter.ToUInt32(data, offset + 0x8);
                Flags = BitConverter.ToUInt32(data, offset + 0xc);
            }
        }

        public static bool ExtractFile(FileDescriptor mpq, string fileName, Stream target)
        {
            if (!target.CanWrite)
                throw new IOException("Cannot write to the output stream.");

            HashTableEntry? t = FindFileInHashTable(mpq, fileName);
            if (t == null) return false;

            HashTableEntry h = t.Value;
            BlockTableEntry b = mpq.BlockTable[h.Index];
            
            if (b.Implode) throw new NotImplementedException("Implosion is not supported yet");
            if (!b.Exists) return false;

            uint fileEncryptionKey = FileKey(fileName, b);

            if (b.Compress)
            {
                uint defaultSectorSize = 512u*(1u << mpq.Header.SectorSizeShift);
                uint sectorCount = b.FileSize/defaultSectorSize
                                           + (b.FileSize%defaultSectorSize == 0u ? 0u : 1u);
                byte[] sectorOffsetTableRaw = new byte[(sectorCount + 1)*4];

                mpq.InputStream.Seek(mpq.HeaderOffset + b.BlockOffset, SeekOrigin.Begin);
                mpq.InputStream.Read(sectorOffsetTableRaw, 0, ((int) sectorCount + 1)*4);
                if (b.Encrypted) Decrypt(ref sectorOffsetTableRaw, fileEncryptionKey - 1);

                uint[] sectorOffsetTable = new uint[sectorCount + 1];
                for (int i = 0; i <= sectorCount; i++)
                {
                    sectorOffsetTable[i] = BitConverter.ToUInt32(sectorOffsetTableRaw, 4*i);
                }

                for (int i = 0; i < sectorCount; i++)
                {
                    mpq.InputStream.Seek(mpq.HeaderOffset + b.BlockOffset + sectorOffsetTable[i], SeekOrigin.Begin);

                    uint sectorUncompressedSize = (i == sectorCount - 1 && b.FileSize%defaultSectorSize != 0) 
                        ? b.FileSize%defaultSectorSize 
                        : defaultSectorSize;

                    uint sectorCompressedSize = sectorOffsetTable[i + 1] - sectorOffsetTable[i];

                    bool sectorCompressed = b.Compress && sectorCompressedSize < sectorUncompressedSize;
                    
                    int compressionFlags = 0;

                    byte[] rawSector = new byte[sectorCompressedSize];
                    mpq.InputStream.Read(rawSector, 0, (int) sectorCompressedSize);

                    if (b.Encrypted) Decrypt(ref rawSector, (uint) (fileEncryptionKey + i));

                    if (sectorCompressed)
                    {
                        compressionFlags = rawSector[0];
                        rawSector = rawSector.Skip(1).ToArray();
                    }

                    if (sectorCompressed)
                    {
                        if ((compressionFlags & 0x08) != 0) //imploded (diablo exclusive probably)
                            throw new NotImplementedException("Implosion is not supported yet.");

                        if ((compressionFlags & 0x02) != 0) //deflated (zlib)
                        {
                            ZlibStream deflateStream = new ZlibStream(new MemoryStream(rawSector), CompressionMode.Decompress);
                            byte[] decompressed = new byte[sectorUncompressedSize];
                            deflateStream.Read(decompressed, 0, (int) sectorUncompressedSize);
                            rawSector = decompressed;
                        }

                        if ((compressionFlags & 0x01) != 0) //huffman
                            throw new NotImplementedException("Huffman encoding is not supported yet.");

                        if ((compressionFlags & 0x80) != 0) //huffman
                            throw new NotImplementedException("IMA ADPCM stereo is not supported yet.");

                        if ((compressionFlags & 0x40) != 0) //huffman
                            throw new NotImplementedException("IMA ADPCM mono is not supported yet.");
                    }

                    target.Write(rawSector, 0, rawSector.Length);
                }
            }
            else
            {
                byte[] data = new byte[b.BlockSize];
                mpq.InputStream.Seek(mpq.HeaderOffset + b.BlockOffset, SeekOrigin.Begin);
                mpq.InputStream.Read(data, 0, (int) b.BlockSize);
                target.Write(data, 0, (int) b.BlockSize);

                target.Close();
            }

            return true;
        }

        public static uint ExtractAll(FileDescriptor mpq, string outputFolder, string[] files = null)
        {
            if (!Directory.Exists(outputFolder))
                throw new FormatException("Output directory is invalid.");

            if (files == null)
            {
                MemoryStream streamListfile = new MemoryStream();
                bool success = ExtractFile(mpq, "(listfile)", streamListfile);
                if (!success) return 0;
                byte[] rawListfile = streamListfile.ToArray();

                string listfile = Encoding.UTF8.GetString(rawListfile);
                files = listfile.Split('\r', '\n').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            }

            uint successCount = 0;
            foreach (string file in files)
            {
                int lastSep = file.LastIndexOf('\\');
                string targetDir = outputFolder + "\\" + file.Substring(0, lastSep != -1 ? lastSep : 0);
                string targetFile = file.Substring(lastSep + 1);

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                FileStream fs = new FileStream(targetDir + "\\" + targetFile, FileMode.Create);
                bool fileSuccess = ExtractFile(mpq, file, fs);
                fs.Close();
                if (fileSuccess) successCount++;
            }

            return successCount;
        }

        public static HashTableEntry[] ReadHashTable(FileDescriptor mpq)
        {
            byte[] raw = new byte[HashTableEntry.Size * mpq.Header.HashTableEntries];
            mpq.InputStream.Seek(mpq.HeaderOffset + mpq.Header.HashTableOffset, SeekOrigin.Begin);
            mpq.InputStream.Read(raw, 0, raw.Length);
            Decrypt(ref raw, Hash("(hash table)", FileHashType));

            HashTableEntry[] table = new HashTableEntry[mpq.Header.HashTableEntries];
            for (int i = 0; i < mpq.Header.HashTableEntries; i++)
            {
                table[i] = new HashTableEntry(raw, (int) HashTableEntry.Size*i);
            }

            return table;
        }

        public static BlockTableEntry[] ReadBlockTable(FileDescriptor mpq)
        {
            byte[] raw = new byte[BlockTableEntry.Size * mpq.Header.BlockTableEntries];
            mpq.InputStream.Seek(mpq.HeaderOffset + mpq.Header.BlockTableOffset, SeekOrigin.Begin);
            mpq.InputStream.Read(raw, 0, raw.Length);
            Decrypt(ref raw, Hash("(block table)", FileHashType));

            BlockTableEntry[] table = new BlockTableEntry[mpq.Header.BlockTableEntries];
            for (int i = 0; i < mpq.Header.BlockTableEntries; i++)
            {
                table[i] = new BlockTableEntry(raw, (int) BlockTableEntry.Size*i);
            }

            return table;
        }

        public static HashTableEntry? FindFileInHashTable(FileDescriptor mpq, string fileName)
        {
            if (mpq.HashTable.Length != mpq.Header.HashTableEntries)
                throw new FormatException("Real hash table size and size from the header do not match.");

            uint fileHashPosition = Hash(fileName, PosHashType) & (mpq.Header.HashTableEntries - 1);
            uint fileHashA = Hash(fileName, AHashType);
            uint fileHashB = Hash(fileName, BHashType);
            
            HashTableEntry? found = null;
            for (int i = 0; found == null && i < mpq.Header.HashTableEntries; i++)
            {
                HashTableEntry entry = mpq.HashTable[(fileHashPosition + i)% mpq.Header.HashTableEntries];
                if (entry.IsDeleted) continue;
                if (entry.IsEmpty) break;

                if (entry.HashA == fileHashA && entry.HashB == fileHashB)
                {
                    found = entry;
                }
            }

            return found;
        }

        public static uint FileKey(string filePath, BlockTableEntry entry)
        {
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

            uint key = Hash(fileName, FileHashType);

            if (entry.AdjustKey)
                key = (key + entry.BlockOffset) ^ entry.FileSize;

            return key;
        }

        public static uint Hash(string s, uint type)
        {
            uint seed1 = 0x7fed7fed;
            uint seed2 = 0xeeeeeeee;

            foreach (char upper in s.Select(char.ToUpper))
            {
                seed1 = CryptTable[type*0x100 + upper] ^ (seed1 + seed2);
                seed2 = upper + seed1 + seed2 + (seed2 << 5) + 3;
            }

            return seed1;
        }

        public static void Encrypt(ref byte[] data, uint key)
        {
            uint seed = 0xeeeeeeee;
            for (int i = 0; i < (data.Length/4); i++)
            {
                seed += CryptTable[0x400 + (key & 0xff)];
                uint c = BitConverter.ToUInt32(data, i*4) ^ (key + seed);

                key = ((~key << 0x15) + 0x11111111) | (key >> 0x0b);
                seed = BitConverter.ToUInt32(data, i*4) + seed + (seed << 5) + 3;

                for (int j = 0; j < 4; j++)
                    data[i*4 + j] = BitConverter.GetBytes(c)[j];
            }
        }

        public static void Decrypt(ref byte[] data, uint key)
        {
            uint seed = 0xeeeeeeee;

            for (int i = 0; i < (data.Length/4); i++)
            {
                seed += CryptTable[0x400 + (key & 0xff)];
                uint c = BitConverter.ToUInt32(data, i*4) ^ (key + seed);

                key = ((~key << 0x15) + 0x11111111) | (key >> 0x0b);
                seed = c + seed + (seed << 5) + 3;

                for (int j = 0; j < 4; j++)
                    data[i*4 + j] = BitConverter.GetBytes(c)[j];
            }
        }
    }
}