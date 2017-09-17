using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

namespace Editor.MapRepresentation
{
    public class Environment
    {
        public Tileset Tileset;
        public bool UseCustomTilesets;
        public string[] GroundTilesets;
        public string[] CliffTilesets;

        public uint Width;
        public uint Height;

        public float CenterOffsetX;
        public float CenterOffsetY;

        public Tilepoint[,] Tiles;

        private Environment()
        {
        }

        public static Environment Read(byte[] raw)
        {
            if (Encoding.UTF8.GetString(raw.Take(4).ToArray()) != "W3E!")
                throw new FormatException("Invalid raw environment data.");
            if (BitConverter.ToUInt32(raw, 0x4) != 11)
                throw new FormatException("Unsupported environment format version.");
            Environment env = new Environment();

            env.Tileset = TilesetHelper.FromCode(Encoding.UTF8.GetChars(raw, 0x8, 1)[0]);
            env.UseCustomTilesets = BitConverter.ToUInt32(raw, 0x9) != 0;

            env.GroundTilesets = new string[BitConverter.ToUInt32(raw, 0xd)];
            for (int i = 0; i < env.GroundTilesets.Length; i++)
                env.GroundTilesets[i] =
                    Encoding.UTF8.GetString(raw.Skip(0x11 + 4*i).Take(4).ToArray());

            env.CliffTilesets = new string[BitConverter.ToUInt32(raw, 0x11 + env.GroundTilesets.Length * 4)];
            for (int i = 0; i < env.CliffTilesets.Length; i++)
                env.CliffTilesets[i] = 
                    Encoding.UTF8.GetString(raw.Skip(0x15 + 4*(env.GroundTilesets.Length + i)).Take(4).ToArray());

            int offset = 0x15 + 4*(env.GroundTilesets.Length + env.CliffTilesets.Length);

            env.Width = BitConverter.ToUInt32(raw, offset);
            env.Height = BitConverter.ToUInt32(raw, offset + 0x4);

            env.CenterOffsetX = BitConverter.ToSingle(raw, offset + 0x8);
            env.CenterOffsetY = BitConverter.ToSingle(raw, offset + 0xc);

            env.Tiles = new Tilepoint[env.Height, env.Width];
            for (int y = 0; y < env.Height; y++)
            {
                for (int x = 0; x < env.Width; x++)
                {
                    env.Tiles[env.Height - y - 1, x] = new Tilepoint(raw, (uint) (offset + 0x10 + 7*(y*env.Width + x)));
                }
            }

            return env;
        }

        public void Write(Stream s)
        {
            s.Write(Encoding.UTF8.GetBytes("W3E!"), 0, 4);
            s.Write(BitConverter.GetBytes(11u), 0, 4);
            s.Write(Encoding.UTF8.GetBytes(new[] { TilesetHelper.ToCode(Tileset) }), 0, 1);
            s.Write(UseCustomTilesets ? BitConverter.GetBytes(1u) : BitConverter.GetBytes(0u), 0, 4);
            s.Write(BitConverter.GetBytes(GroundTilesets.Length), 0, 4);
            foreach (string tileset in GroundTilesets)
                s.Write(Encoding.UTF8.GetBytes(tileset), 0, 4);
            s.Write(BitConverter.GetBytes(CliffTilesets.Length), 0, 4);
            foreach (string tileset in CliffTilesets)
                s.Write(Encoding.UTF8.GetBytes(tileset), 0, 4);
            s.Write(BitConverter.GetBytes(Width), 0, 4);
            s.Write(BitConverter.GetBytes(Height), 0, 4);
            s.Write(BitConverter.GetBytes(CenterOffsetX), 0, 4);
            s.Write(BitConverter.GetBytes(CenterOffsetY), 0, 4);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tiles[Height - y - 1, x].Write(s);
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Tilepoint
    {
        private byte b0;
        private byte b1;
        private byte b2;
        private byte b3;
        private byte b4;
        private byte b5;
        private byte b6;

        public static short GroundHeightMin = short.MinValue;
        public static short GroundHeightMax = short.MaxValue;
        public short GroundHeight
        {
            get { return (short) ((b1 << 8) | b0); }
            set
            {
                b0 = (byte) (value & 0xFF);
                b1 = (byte) ((value & 0xFF00) >> 8);
            }
        }

        public float FinalHeight
        {
            get { return ToFinalHeight(LayerHeight, GroundHeight); }
            set { GroundHeight = ToGroundHeight(LayerHeight, value); }
        }
        public float FinalWater => (WaterLevel - 0x2000)/4f - 89.6f;

        public static short WaterLevelMin = 0;
        public static short WaterLevelMax = 0x3fff;
        public short WaterLevel
        {
            get { return (short) (((b3 << 8) | b2) & 0x3fff); }
            set
            {
                b2 = (byte) (value & 0xff);
                b3 = (byte) (((value & 0x3f00) >> 8) | (b3 & 0x40));
            }
        }
        
        public bool Boundary
        {
            get { return (b3 & 0x40) != 0; }
            set
            {
                if (value) b3 |= 0x40;
                else b3 &= 0x3F;
            }
        }

        public bool Ramp
        {
            get { return (b4 & 0x10) != 0; }
            set
            {
                if (value) b4 |= 0x10;
                else b4 &= 0xef;
            }
        }

        public bool Blight
        {
            get { return (b4 & 0x20) != 0; }
            set
            {
                if (value) b4 |= 0x20;
                else b4 &= 0xdf;
            }
        }

        public bool Water
        {
            get { return (b4 & 0x40) != 0; }
            set
            {
                if (value) b4 |= 0x40;
                else b4 &= 0xbf;
            }
        }

        public bool CamBoundary
        {
            get { return (b4 & 0x80) != 0; }
            set
            {
                if (value) b4 |= 0x80;
                else b4 &= 0x7f;
            }
        }

        public byte GroundTexture
        {
            get { return (byte) (b4 & 0xf); }
            set { b4 = (byte) ((b4 & 0xf0) | (value & 0xf)); }
        }

        public static byte TextureDetailsMin = 0;
        public static byte TextureDetailsMax = 0xff;
        public byte TextureDetails
        {
            get { return b5; }
            set { b5 = value; }
        }

        public static byte CliffTextureMin = 0;
        public static byte CliffTextureMax = 0xf;
        public byte CliffTexture
        {
            get { return (byte)((b6 & 0xf0) >> 4); }
            set { b6 = (byte)((b6 & 0xf) | ((value & 0xf) << 4)); }
        }

        public static byte LayerHeightMin = 0;
        public static byte LayerHeightMax = 0xf;
        public byte LayerHeight
        {
            get { return (byte)(b6 & 0xf); }
            set { b6 = (byte)((b6 & 0xf0) | (value & 0xf)); }
        }

        public Tilepoint(byte[] data, uint offset)
        {
            b0 = data[offset + 0x0];
            b1 = data[offset + 0x1];
            b2 = data[offset + 0x2];
            b3 = data[offset + 0x3];
            b4 = data[offset + 0x4];
            b5 = data[offset + 0x5];
            b6 = data[offset + 0x6];
        }

        public void Write(Stream s)
        {
            s.WriteByte(b0);
            s.WriteByte(b1);
            s.WriteByte(b2);
            s.WriteByte(b3);
            s.WriteByte(b4);
            s.WriteByte(b5);
            s.WriteByte(b6);
        }

        public static readonly Func<byte, short, float> ToFinalHeight = (lh, gh) => (gh - 0x2000 + (lh - 2)*0x200)/4f;
        public static readonly Func<byte, float, short> ToGroundHeight = (lh, fh) => (short) (4*fh + 0x2000 - (lh - 2)*0x200);
    }

    public enum Tileset
    {
        Ashenvale,
        Barrens,
        LordaeronFall,
        LordaeronSummer,
        Northrend,
        Village,
        LordaeronWinter,
        CityDalaran,
        CityLordaeron
    }

    public static class TilesetHelper
    {
        public static Tileset FromCode(char a)
        {
            switch (a)
            {
                case 'A': return Tileset.Ashenvale;
                case 'B': return Tileset.Barrens;
                case 'F': return Tileset.LordaeronFall;
                case 'L': return Tileset.LordaeronSummer;
                case 'N': return Tileset.Northrend;
                case 'V': return Tileset.Village;
                case 'W': return Tileset.LordaeronWinter;
                case 'X': return Tileset.CityDalaran;
                case 'Y': return Tileset.CityLordaeron;
                default:
                    throw new FormatException("Invalid code-tileset conversion.");
            }
        }
        
        public static char ToCode(Tileset a)
        {
            switch (a)
            {
                case Tileset.Ashenvale: return 'A';
                case Tileset.Barrens: return 'B';
                case Tileset.LordaeronFall: return 'F';
                case Tileset.LordaeronSummer: return 'L';
                case Tileset.Northrend: return 'N';
                case Tileset.Village: return 'V';
                case Tileset.LordaeronWinter: return 'W';
                case Tileset.CityDalaran: return 'X';
                case Tileset.CityLordaeron: return 'Y';
                default:
                    throw new FormatException("Invalid tileset-code conversion.");
            }
        }
    }
}
