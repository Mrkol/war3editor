using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using Editor.ModelRepresentation.Chunks;
using Editor.ModelRepresentation.Objects;

namespace Editor.ModelRepresentation
{
    static class Parser
    {
        public static ModelX Read(byte[] data)
        {
            ModelX mdx = new ModelX();

            int offset = 0;

            ReadTag(data, ref offset, Chunk.MDLX);


            while (offset < data.Length)
            {
                uint tag = BitConverter.ToUInt32(data, offset);
                int size = (int) BitConverter.ToUInt32(data, offset + 4);
                offset += 8; //size values greater than int.maxvalue are undefined behaviour

                switch (tag)
                {
                    case Chunk.VERS:
                        mdx.CVersion = ReadStruct<VERS>(data, ref offset);
                        break;

                    case Chunk.MODL:
                        mdx.CModel = ReadStruct<MODL>(data, ref offset);
                        break;

                    case Chunk.SEQS:
                        mdx.CSequences = ReadSEQS(data, ref offset, size);
                        break;

                    case Chunk.GLBS:
                        mdx.CGlobalSequences = ReadGLBS(data, ref offset, size);
                        break;

                    default:
                        offset += size;
                        break;
                }
            }


            return mdx;
        }

        public delegate T Reader<T>(byte[] data, ref int offset);
        public delegate T ReaderSized<T>(byte[] data, ref int offset, int size);

        public static T[] ReadArray<T>(byte[] data, ref int offset, int size, Reader<T> elementReader)
        {
            int oldOffset = offset;
            T[] array = new T[0];
            while (offset < oldOffset + size)
            {
                //TODO: optimize
                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = elementReader(data, ref offset);
            }
            return array;
        }

        public static SEQS ReadSEQS(byte[] data, ref int offset, int size)
        {
            SEQS seqs;
            seqs.Sequences = ReadArray(data, ref offset, size, ReadSequence);
            return seqs;
        }

        public static GLBS ReadGLBS(byte[] data, ref int offset, int size)
        {
            GLBS glbs;
            glbs.Sequences = ReadArray(data, ref offset, size, ReadGlobalSequence);
            return glbs;
        }

        public static Sequence ReadSequence(byte[] data, ref int offset)
        {
            Sequence sequence;
            sequence.Name = ReadString(data, ref offset, 0x50);
            unsafe
            {
                sequence.Interval[0] = BitConverter.ToUInt32(data, offset + 0x50);
                sequence.Interval[1] = BitConverter.ToUInt32(data, offset + 0x54);                
            }
            sequence.MoveSpeed = BitConverter.ToSingle(data, offset + 0x58);
            sequence.Flags = BitConverter.ToUInt32(data, offset + 0x5c);
            sequence.Rarity = BitConverter.ToSingle(data, offset + 0x60);
            sequence.SyncPoint = BitConverter.ToUInt32(data, offset + 0x64);
            sequence.Extent = ReadExtent(data, ref offset);
            offset += 0x4*6;
            return sequence;
        }

        public static Reader<GlobalSequence> ReadGlobalSequence = ReadStruct<GlobalSequence>;

        public static Reader<Extent> ReadExtent = ReadStruct<Extent>;

        public static void ReadTag(byte[] data, ref int offset, uint tag)
        {
            if (BitConverter.ToUInt32(data, offset) != tag)
                throw new FormatException("Expected a " + TagToString(tag) + " tag at " + offset.ToString("X8"));
            offset += 4;
        }

        public static string ReadString(byte[] data, ref int offset, int count)
        {
            string s = Encoding.UTF8.GetString(data, offset, count);
            s = s.Substring(0, s.IndexOf('\0'));
            offset += count;
            return s;
        }

        public static string TagToString(uint tag)
        {
            return Encoding.UTF8.GetString(BitConverter.GetBytes(tag), 0, 4);
        }
        
        public static T ReadStruct<T>(byte[] data, ref int offset)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T item = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject() + offset, typeof (T));
            handle.Free();
            offset += Marshal.SizeOf(typeof (T));
            return item;
        }
    }


    static class Chunk
    {
        public const uint MDLX = 0x4d444c58;

        public const uint VERS = 0x56455253;
        public const uint MODL = 0x4d4f444c;
        public const uint SEQS = 0x53455153;
        public const uint GLBS = 0x474c4253;
        public const uint TEXS = 0x54455853;
        public const uint SNDS = 0x534e4453;
        public const uint MTLS = 0x4d544c53;
        public const uint TXAN = 0x5458414e;
        public const uint GEOS = 0x47454f53;
        public const uint GEOA = 0x47454f41;
        public const uint BONE = 0x424f4e45;
        public const uint LITE = 0x4c495445;
        public const uint HELP = 0x48454c50;
        public const uint ATCH = 0x41544348;
        public const uint PIVT = 0x50495654;
        public const uint PREM = 0x5052454d;
        public const uint PRE2 = 0x50524532;
        public const uint RIBB = 0x52494242;
        public const uint EVTS = 0x45565453;
        public const uint CAMS = 0x43414d53;
        public const uint CLID = 0x434c4944;
    }
}
