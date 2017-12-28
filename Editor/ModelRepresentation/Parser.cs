using System;
using System.Text;
using System.Runtime.InteropServices;
using Editor.ModelRepresentation.Chunks;
using Editor.ModelRepresentation.Tracks;

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
                uint size = ReadUint(data, ref offset);
                //size values greater than int.maxvalue are undefined behaviour

                switch (tag)
                {
                    case Chunk.VERS:
                        mdx.CVersion = ReadStruct<VERS>(data, ref offset);
                        break;

                    case Chunk.MODL:
                        mdx.CModel = ReadMODL(data, ref offset, size);
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

                    case Chunk.TEXS:
                        mdx.CTextures = ReadTEXS(data, ref offset, size);
                        break;

                    case Chunk.MTLS:
                        mdx.CMaterials = ReadMTLS(data, ref offset, size);
                        break;

                    case Chunk.BONE:
                        mdx.CBones = ReadBONE(data, ref offset, size);
                        break;

                    case Chunk.PIVT:
                        mdx.CPivots = ReadPIVT(data, ref offset, size);
                        break;

                    default:
                        offset += (int) size;
                        break;
                }
            }


            return mdx;
        }

        public delegate T Reader<T>(byte[] data, ref int offset);
        public delegate T ReaderSized<T>(byte[] data, ref int offset, uint size);

        public static void ReadTag(byte[] data, ref int offset, uint tag)
        {
            if (!TryReadTag(data, ref offset, tag))
                throw new FormatException("Expected a " + TagToString(tag) + " tag at " + offset.ToString("X8"));
        }

        public static bool TryReadTag(byte[] data, ref int offset, uint tag)
        {
            if (BitConverter.ToUInt32(data, offset) != tag)
                return false;
            offset += 4;
            return true;
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

        public static int ReadInt(byte[] data, ref int offset)
        {
            int n = BitConverter.ToInt32(data, offset);
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
        
        public static T ReadStruct<T>(byte[] data, ref int offset) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T item = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject() + offset, typeof (T));
            handle.Free();
            offset += Marshal.SizeOf(typeof (T));
            return item;
        }

        public static T[] ReadArray<T>(byte[] data, ref int offset, uint size, Reader<T> elementReader = null) 
            where T : struct
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

        public static T[] ReadFixedArray<T>(byte[] data, ref int offset, uint count, Reader<T> elementReader)
        {
            if (elementReader == null) 
                throw new FormatException("Can't pass a null reader to ReadFixedArray.");

            T[] array = new T[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = elementReader(data, ref offset);
            }
            return array;
        }

        public static T? ReadChunk<T>(byte[] data, ref int offset, ReaderSized<T> reader) where T : struct
        {
            bool tagPresent = TryReadTag(data, ref offset, Chunk.FromType<T>());
            if (!tagPresent) return null;
            uint size = ReadUint(data, ref offset);
            return reader(data, ref offset, size);
        }
    }


    static class Chunk
    {
        public const uint MDLX = 0x584c444d;

        public const uint VERS = 0x53524556;
        public const uint MODL = 0x4c444f4d;
        public const uint SEQS = 0x53514553;
        public const uint GLBS = 0x53424c47;
        public const uint TEXS = 0x53584554;
        public const uint SNDS = 0x53444e53;
        public const uint MTLS = 0x534c544d;
        public const uint TXAN = 0x4e415854;
        public const uint GEOS = 0x534f4547;
        public const uint GEOA = 0x414f4547;
        public const uint BONE = 0x454e4f42;
        public const uint LITE = 0x4554494c;
        public const uint HELP = 0x504c4548;
        public const uint ATCH = 0x48435441;
        public const uint PIVT = 0x54564950;
        public const uint PREM = 0x4d455250;
        public const uint PRE2 = 0x32455250;
        public const uint RIBB = 0x42424952;
        public const uint EVTS = 0x53545645;
        public const uint CAMS = 0x534d4143;
        public const uint CLID = 0x44494c43;
        public const uint LAYS = 0x5359414c;
        public const uint VRTX = 0x58545256;
        public const uint NRMS = 0x534d524e;
        public const uint PTYP = 0x50595450;
        public const uint PCNT = 0x544e4350;
        public const uint PVTX = 0x58545650;
        public const uint GNDX = 0x58444e47;
        public const uint MTGC = 0x4347544d;
        public const uint MATS = 0x5354414d;
        public const uint UVAS = 0x53415655;
        public const uint UVBS = 0x53425655;

        public const uint KGRT = 0x5452474b;
        public const uint KGSC = 0x4353474b;
        public const uint KGTR = 0x5254474b;
        public const uint KMTF = 0x46544d4b;
        public const uint KMTA = 0x41544d4b;
        public const uint KTAT = 0x5441544b;
        public const uint KTAR = 0x5241544b;
        public const uint KTAS = 0x5341544b;
        public const uint KGAO = 0x4f41474b;
        public const uint KGAC = 0x4341474b;
        public const uint KLAS = 0x53414c4b;
        public const uint KLAE = 0x45414c4b;
        public const uint KLAC = 0x43414c4b;
        public const uint KLAI = 0x49414c4b;
        public const uint KLBI = 0x49424c4b;
        public const uint KLBC = 0x43424c4b;
        public const uint KLAV = 0x56414c4b;
        public const uint KATV = 0x5654414b;
        public const uint KPEE = 0x4545504b;
        public const uint KPEG = 0x4745504b;
        public const uint KPLN = 0x4e4c504b;
        public const uint KPLT = 0x544c504b;
        public const uint KPEL = 0x4c45504b;
        public const uint KPES = 0x5345504b;
        public const uint KPEV = 0x5645504b;
        public const uint KP2E = 0x4532504b;
        public const uint KP2G = 0x4732504b;
        public const uint KP2L = 0x4c32504b;
        public const uint KP2S = 0x5332504b;
        public const uint KP2V = 0x5632504b;
        public const uint KP2R = 0x5232504b;
        public const uint KP2N = 0x4e32504b;
        public const uint KP2W = 0x5732504b;
        public const uint KRVS = 0x5356524b;
        public const uint KRHA = 0x4148524b;
        public const uint KRHB = 0x4248524b;
        public const uint KRAL = 0x4c41524b;
        public const uint KRCO = 0x4f43524b;
        public const uint KRTX = 0x5854524b;
        public const uint KCTR = 0x5254434b;
        public const uint KCRL = 0x4c52434b;
        public const uint KTTR = 0x5254544b;

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
            if (typeof(T) == typeof(LAYS)) return Chunk.LAYS;
            if (typeof(T) == typeof(VRTX)) return Chunk.VRTX;
            if (typeof(T) == typeof(NRMS)) return Chunk.NRMS;
            if (typeof(T) == typeof(PTYP)) return Chunk.PTYP;
            if (typeof(T) == typeof(PCNT)) return Chunk.PCNT;
            if (typeof(T) == typeof(PVTX)) return Chunk.PVTX;
            if (typeof(T) == typeof(GNDX)) return Chunk.GNDX;
            if (typeof(T) == typeof(MTGC)) return Chunk.MTGC;
            if (typeof(T) == typeof(MATS)) return Chunk.MATS;
            if (typeof(T) == typeof(UVAS)) return Chunk.UVAS;
            if (typeof(T) == typeof(UVBS)) return Chunk.UVBS;

            if (typeof(T) == typeof(KGRT)) return Chunk.KGRT;
            if (typeof(T) == typeof(KGSC)) return Chunk.KGSC;
            if (typeof(T) == typeof(KGTR)) return Chunk.KGTR;
            if (typeof(T) == typeof(KMTF)) return Chunk.KMTF;
            if (typeof(T) == typeof(KMTA)) return Chunk.KMTA;
            if (typeof(T) == typeof(KTAT)) return Chunk.KTAT;
            if (typeof(T) == typeof(KTAR)) return Chunk.KTAR;
            if (typeof(T) == typeof(KTAS)) return Chunk.KTAS;
            if (typeof(T) == typeof(KGAO)) return Chunk.KGAO;
            if (typeof(T) == typeof(KGAC)) return Chunk.KGAC;
            if (typeof(T) == typeof(KLAS)) return Chunk.KLAS;
            if (typeof(T) == typeof(KLAE)) return Chunk.KLAE;
            if (typeof(T) == typeof(KLAC)) return Chunk.KLAC;
            if (typeof(T) == typeof(KLAI)) return Chunk.KLAI;
            if (typeof(T) == typeof(KLBI)) return Chunk.KLBI;
            if (typeof(T) == typeof(KLBC)) return Chunk.KLBC;
            if (typeof(T) == typeof(KLAV)) return Chunk.KLAV;
            if (typeof(T) == typeof(KATV)) return Chunk.KATV;
            if (typeof(T) == typeof(KPEE)) return Chunk.KPEE;
            if (typeof(T) == typeof(KPEG)) return Chunk.KPEG;
            if (typeof(T) == typeof(KPLN)) return Chunk.KPLN;
            if (typeof(T) == typeof(KPLT)) return Chunk.KPLT;
            if (typeof(T) == typeof(KPEL)) return Chunk.KPEL;
            if (typeof(T) == typeof(KPES)) return Chunk.KPES;
            if (typeof(T) == typeof(KPEV)) return Chunk.KPEV;
            if (typeof(T) == typeof(KP2E)) return Chunk.KP2E;
            if (typeof(T) == typeof(KP2G)) return Chunk.KP2G;
            if (typeof(T) == typeof(KP2L)) return Chunk.KP2L;
            if (typeof(T) == typeof(KP2S)) return Chunk.KP2S;
            if (typeof(T) == typeof(KP2V)) return Chunk.KP2V;
            if (typeof(T) == typeof(KP2R)) return Chunk.KP2R;
            if (typeof(T) == typeof(KP2N)) return Chunk.KP2N;
            if (typeof(T) == typeof(KP2W)) return Chunk.KP2W;
            if (typeof(T) == typeof(KRVS)) return Chunk.KRVS;
            if (typeof(T) == typeof(KRHA)) return Chunk.KRHA;
            if (typeof(T) == typeof(KRHB)) return Chunk.KRHB;
            if (typeof(T) == typeof(KRAL)) return Chunk.KRAL;
            if (typeof(T) == typeof(KRCO)) return Chunk.KRCO;
            if (typeof(T) == typeof(KRTX)) return Chunk.KRTX;
            if (typeof(T) == typeof(KCTR)) return Chunk.KCTR;
            if (typeof(T) == typeof(KCRL)) return Chunk.KCRL;
            if (typeof(T) == typeof(KTTR)) return Chunk.KTTR;

            Console.WriteLine("Warning: tag for type {0} is uknown.", typeof(T));
            return 0;
        }
    }
}
