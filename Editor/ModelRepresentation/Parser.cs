using System;
using System.Runtime.InteropServices;
using System.Text;
using Editor.ModelRepresentation.Chunks;

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

                int oldOffset = offset;
                switch (tag)
                {
                    case Chunk.VERS:
                        mdx.CVersion = ReadStruct<VERS>(data, ref offset);
                        break;

                    case Chunk.MODL:
                        mdx.CModel = ReadStruct<MODL>(data, ref offset);
                        break;

                    case Chunk.SEQS:
                        while (offset < oldOffset + size)
                        {
                            
                        }
                        break;

                    default:
                        offset += size;
                        break;
                }
            }


            return mdx;
        }

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


    internal static class Chunk
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
