using Editor.ModelRepresentation.Chunks;
using Editor.ModelRepresentation.Objects;
using Editor.ModelRepresentation.Tracks;
using OpenTK;

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
            Texture texture = new Texture();
            texture.ReplaceableId = ReadUint(data, ref offset);
            texture.FileName = ReadString(data, ref offset, 260);
            texture.Flags = ReadUint(data, ref offset);
            return texture;
        }

        public static Material ReadMaterial(byte[] data, ref int offset)
        {
            int prevOffset = offset;
        	Material material;
        	material.InclusiveSize = ReadUint(data, ref offset);
        	material.PriorityPlane = ReadUint(data, ref offset);
        	material.Flags = ReadUint(data, ref offset);
        	material.Lays = ReadChunk(data, ref offset, ReadLAYS).Value;
            offset = prevOffset + (int) material.InclusiveSize;
        	return material;
        }

        public static Layer ReadLayer(byte[] data, ref int offset)
        {
            int prevOffset = offset;
        	Layer layer = new Layer();
        	layer.InclusiveSize = ReadUint(data, ref offset);
        	layer.FilterMode = ReadUint(data, ref offset);
        	layer.ShadingFlags = ReadUint(data, ref offset);
        	layer.TextureId = ReadUint(data, ref offset);
        	layer.TextureAnimationId = ReadUint(data, ref offset);
        	layer.CoordId = ReadUint(data, ref offset);
        	layer.Alpha = ReadFloat(data, ref offset);

            layer.Kmta = ReadTracksChunk<KMTA, float>(data, ref offset);
            layer.Kmtf = ReadTracksChunk<KMTF, uint>(data, ref offset);

            offset = prevOffset + (int) layer.InclusiveSize;
        	return layer;
        }

        public static Bone ReadBone(byte[] data, ref int offset)
        {
        	Bone bone = new Bone();
        	bone.Node = ReadNode(data, ref offset);
        	bone.GeosetId = ReadUint(data, ref offset);
        	bone.GeosetAnimationId = ReadUint(data, ref offset);
        	return bone;
        }

        public static Geoset ReadGeoset(byte[] data, ref int offset)
        {
            int prevOffset = offset;
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

            offset = prevOffset + (int) geoset.InclusiveSize;
            return geoset;
        }

        public static Node ReadNode(byte[] data, ref int offset)
        {
            int prevOffset = offset;
        	Node node = new Node();
        	node.InclusiveSize = ReadUint(data, ref offset);
        	node.Name = ReadString(data, ref offset, 80);
        	node.ObjectId = ReadUint(data, ref offset);
        	node.ParentId = ReadUint(data, ref offset);
        	node.Flags = ReadUint(data, ref offset);

            node.Kgtr = ReadTracksChunk<KGTR, Vector3>(data, ref offset);
            node.Kgsc = ReadTracksChunk<KGSC, Vector3>(data, ref offset);
            node.Kgrt = ReadTracksChunk<KGRT, Vector4>(data, ref offset);

            offset = prevOffset + (int) node.InclusiveSize;
        	return node;
        }

        public static Reader<GlobalSequence> ReadGlobalSequence = ReadStruct<GlobalSequence>;

        public static Reader<Extent> ReadExtent = ReadStruct<Extent>;
    }
}