using Editor.ModelRepresentation.Chunks;
using Editor.ModelRepresentation.Objects;

namespace Editor.ModelRepresentation
{
    static partial class Parser
    {
        public static Sequence ReadSequence(byte[] data, ref int offset)
        {
            Sequence sequence;
            sequence.Name = ReadString(data, ref offset, 0x50);
            unsafe
            {
                sequence.Interval[0] = ReadUint(data, ref offset);
                sequence.Interval[1] = ReadUint(data, ref offset);
            }
            sequence.MoveSpeed = ReadFloat(data, ref offset);
            sequence.Flags = ReadUint(data, ref offset);
            sequence.Rarity = ReadFloat(data, ref offset);
            sequence.SyncPoint = ReadUint(data, ref offset);
            sequence.Extent = ReadExtent(data, ref offset);
            return sequence;
        }

        public static Texture ReadTexture(byte[] data, ref int offset)
        {
            Texture texture;
            texture.ReplaceableId = ReadUint(data, ref offset);
            texture.FileName = ReadString(data, ref offset, 260);
            texture.Flags = ReadUint(data, ref offset);
            return texture;
        }

        public static Geoset ReadGeoset(byte[] data, ref int offset)
        {
            Geoset geoset = new Geoset();
            geoset.InclusiveSize = ReadUint(data, ref offset);
            //TODO: graceful exception when any of those are missing
            geoset.Vrtx = ReadChunk<VRTX>(data, ref offset, ReadVRTX).Value;
            geoset.Nrms = ReadChunk<NRMS>(data, ref offset, ReadNRMS).Value;
            geoset.Ptyp = ReadChunk<PTYP>(data, ref offset, ReadPTYP).Value;
            geoset.Pcnt = ReadChunk<PCNT>(data, ref offset, ReadPCNT).Value;
            geoset.Pvtx = ReadChunk<PVTX>(data, ref offset, ReadPVTX).Value;
            geoset.Gndx = ReadChunk<GNDX>(data, ref offset, ReadGNDX).Value;
            geoset.Mtgc = ReadChunk<MTGC>(data, ref offset, ReadMTGC).Value;
            geoset.Mats = ReadChunk<MATS>(data, ref offset, ReadMATS).Value;

            geoset.MaterialId = ReadUint(data, ref offset);
            geoset.SelectionGroup = ReadUint(data, ref offset);
            geoset.SelectionFlags = ReadUint(data, ref offset);
            geoset.Extent = ReadExtent(data, ref offset);
            geoset.ExtentsCount = ReadUint(data, ref offset);
            geoset.Extents = ReadFixedArray(data, ref offset, geoset.ExtentsCount, ReadExtent);
            geoset.Uvas = ReadChunk<UVAS>(data, ref offset, ReadUVAS).Value;
            return geoset;
        }

        public static Reader<GlobalSequence> ReadGlobalSequence = ReadStruct<GlobalSequence>;

        public static Reader<Extent> ReadExtent = ReadStruct<Extent>;
    }
}