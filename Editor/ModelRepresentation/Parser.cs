using System;
using System.Text;
using System.Runtime.InteropServices;
using Editor.ModelRepresentation.Chunks;

namespace Editor.ModelRepresentation
{
    static partial class Parser
    {
        public static ModelX Read(byte[] data)
        {
            ModelX mdx = new ModelX();

            int offset = 0;

            ReadTag(data, ref offset, Chunk.MDLX);


            while (offset < data.Length)
            {
                uint tag = ReadUint(data, ref offset);
                int size = (int) ReadUint(data, ref offset);
                //size values greater than int.maxvalue are undefined behaviour

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

                    case Chunk.GEOS:
                        mdx.CGeosets = ReadGEOS(data, ref offset, size);
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

        public static uint ReadUint(byte[] data, ref int offset)
        {
            uint n = BitConverter.ToUInt32(data, offset);
            offset += 4;
            return n;
        }

        public static ushort ReadUshort(byte[] data, ref int offset)
        {
            ushort n = BitConverter.ToUInt16(data, offset);
            offset += 2;
            return n;
        }

        public static byte ReadByte(byte[] data, ref int offset)
        {
            byte n = data[offset];
            offset += 1;
            return n;
        }

        public static float ReadFloat(byte[] data, ref int offset)
        {
            float n = BitConverter.ToSingle(data, offset);
            offset += 4;
            return n;
        }
        
        public static T ReadStruct<T>(byte[] data, ref int offset)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T item = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject() + offset, typeof (T));
            handle.Free();
            offset += Marshal.SizeOf(typeof (T));
            return item;
        }

        public static T[] ReadArray<T>(byte[] data, ref int offset, int size, Reader<T> elementReader = null)
        {
            if (elementReader == null) elementReader = ReadStruct<T>;
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

        public static T[] ReadFixedArray<T>(byte[] data, ref int offset, uint count, Reader<T> elementReader = null)
        {
            if (elementReader == null) elementReader = ReadStruct<T>;
            T[] array = new T[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = elementReader(data, ref offset);
            }
            return array;
        }

        public static T? ReadChunk<T>(byte[] data, ref int offset, ReaderSized<T> reader) where T : struct
        {
            uint tag = ReadUint(data, ref offset);
            if (tag != Chunk.FromType<T>()) 
            {
                offset -= 4;
                return null;
            }
            int size = (int) ReadUint(data, ref offset);
            return reader(data, ref offset, size);
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

        public static uint FromType<T>()
        {
            if (typeof(T) == typeof(VERS)) return Chunk.VERS;
            if (typeof(T) == typeof(MODL)) return Chunk.MODL;
            if (typeof(T) == typeof(SEQS)) return Chunk.SEQS;
            if (typeof(T) == typeof(GLBS)) return Chunk.GLBS;
            if (typeof(T) == typeof(TEXS)) return Chunk.TEXS;
            if (typeof(T) == typeof(SNDS)) return Chunk.SNDS;
            if (typeof(T) == typeof(MTLS)) return Chunk.MTLS;
            if (typeof(T) == typeof(TXAN)) return Chunk.TXAN;
            if (typeof(T) == typeof(GEOS)) return Chunk.GEOS;
            if (typeof(T) == typeof(GEOA)) return Chunk.GEOA;
            if (typeof(T) == typeof(BONE)) return Chunk.BONE;
            if (typeof(T) == typeof(LITE)) return Chunk.LITE;
            if (typeof(T) == typeof(HELP)) return Chunk.HELP;
            if (typeof(T) == typeof(ATCH)) return Chunk.ATCH;
            if (typeof(T) == typeof(PIVT)) return Chunk.PIVT;
            if (typeof(T) == typeof(PREM)) return Chunk.PREM;
            if (typeof(T) == typeof(PRE2)) return Chunk.PRE2;
            if (typeof(T) == typeof(RIBB)) return Chunk.RIBB;
            if (typeof(T) == typeof(EVTS)) return Chunk.EVTS;
            if (typeof(T) == typeof(CAMS)) return Chunk.CAMS;
            if (typeof(T) == typeof(CLID)) return Chunk.CLID;

            return 0;
        }
    }
}
