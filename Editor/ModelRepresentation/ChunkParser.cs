using Editor.ModelRepresentation.Chunks;
using OpenTK;

namespace Editor.ModelRepresentation
{
    static partial class Parser
    {
    	public static MODL ReadMODL(byte[] data, ref int offset, uint size)
    	{
    		MODL modl;
    		modl.Name = ReadString(data, ref offset, 80);
    		modl.AnimationFileName = ReadString(data, ref offset, 260);
    		modl.Extent = ReadExtent(data, ref offset);
    		modl.BlendTime = ReadUint(data, ref offset);
    		return modl;
    	}

        public static SEQS ReadSEQS(byte[] data, ref int offset, uint size)
        {
            SEQS seqs;
            seqs.Sequences = ReadArray(data, ref offset, size, ReadSequence);
            return seqs;
        }

        public static GLBS ReadGLBS(byte[] data, ref int offset, uint size)
        {
            GLBS glbs;
            glbs.Sequences = ReadArray(data, ref offset, size, ReadGlobalSequence);
            return glbs;
        }

        public static TEXS ReadTEXS(byte[] data, ref int offset, uint size)
        {
            TEXS texs;
            texs.Textures = ReadArray(data, ref offset, size, ReadTexture);
            return texs;
        }

        public static GEOS ReadGEOS(byte[] data, ref int offset, uint size)
        {
            GEOS geos;
            geos.Geosets = ReadArray(data, ref offset, size, ReadGeoset);
            return geos;
        }

        public static MTLS ReadMTLS(byte[] data, ref int offset, uint size)
        {
        	MTLS mtls;
        	mtls.Materials = ReadArray(data, ref offset, size, ReadMaterial);
        	return mtls;
        }

        public static BONE ReadBONE(byte[] data, ref int offset, uint size)
        {
        	BONE bone;
        	bone.Bones = ReadArray(data, ref offset, size, ReadBone);
        	return bone;
        }

        public static PIVT ReadPIVT(byte[] data, ref int offset, uint size)
        {
        	PIVT pivt;
        	pivt.Points = ReadArray(data, ref offset, size, ReadStruct<Vector3>);
        	return pivt;
        }

        public static VRTX ReadVRTX(byte[] data, ref int offset, uint size)
        {
            VRTX vrtx;
            vrtx.VerticesCount = size;
            vrtx.Vertices = ReadFixedArray<Vector3>(data, ref offset, vrtx.VerticesCount, ReadStruct<Vector3>);
            return vrtx;
        }

        public static NRMS ReadNRMS(byte[] data, ref int offset, uint size)
        {
            NRMS nrms;
            nrms.NormalsCount = size;
            nrms.Normals = ReadFixedArray<Vector3>(data, ref offset, nrms.NormalsCount, ReadStruct<Vector3>);
            return nrms;
        }

        public static PTYP ReadPTYP(byte[] data, ref int offset, uint size)
        {
            PTYP ptyp;
            ptyp.FaceGroupsCount = size;
            ptyp.FaceGroupPrimitiveTypes = ReadFixedArray<uint>(data, ref offset, ptyp.FaceGroupsCount, ReadUint);
            return ptyp;
        }

        public static PCNT ReadPCNT(byte[] data, ref int offset, uint size)
        {
            PCNT pcnt;
            pcnt.FaceGroupsCount = size;
            pcnt.FaceGroupPrimitiveCounts = ReadFixedArray(data, ref offset, pcnt.FaceGroupsCount, ReadUint);
            return pcnt;
        }

        public static PVTX ReadPVTX(byte[] data, ref int offset, uint size)
        {
            PVTX pvtx;
            pvtx.FaceGroupsCount = size;
            pvtx.FaceGroups = ReadFixedArray(data, ref offset, pvtx.FaceGroupsCount, ReadUshort);
            return pvtx;
        }

        public static GNDX ReadGNDX(byte[] data, ref int offset, uint size)
        {
            GNDX gndx;
            gndx.VertexGroupsCount = size;
            gndx.VertexGroups = ReadFixedArray(data, ref offset, gndx.VertexGroupsCount, ReadByte);
            return gndx;
        }

        public static MTGC ReadMTGC(byte[] data, ref int offset, uint size)
        {
            MTGC mtgc;
            mtgc.MatrixGroupsCount = size;
            mtgc.MatrixGroups = ReadFixedArray(data, ref offset, mtgc.MatrixGroupsCount, ReadUint);
            return mtgc;
        }

        public static MATS ReadMATS(byte[] data, ref int offset, uint size)
        {
            MATS mats;
            mats.MatrixIndicesCount = size;
            mats.MatrixIndices = ReadFixedArray(data, ref offset, mats.MatrixIndicesCount, ReadUint);
            return mats;
        }

        public static UVAS ReadUVAS(byte[] data, ref int offset, uint size)
        {
            UVAS uvas;
            uvas.TextureCoordinateSetsCount = size;
            uvas.TextureCoordinateSets = ReadFixedArray(data, ref offset, uvas.TextureCoordinateSetsCount, 
                    (byte[] d, ref int o) => ReadChunk<UVBS>(d, ref o, ReadUVBS).Value);
            return uvas;
        }

        public static UVBS ReadUVBS(byte[] data, ref int offset, uint size)
        {
            UVBS uvbs;
            uvbs.TextureCoordinatesCount = size;
            uvbs.TextureCoordinates = ReadFixedArray(data, ref offset, uvbs.TextureCoordinatesCount, ReadStruct<Vector2>);
            return uvbs;
        }

        public static LAYS ReadLAYS(byte[] data, ref int offset, uint size)
        {
        	LAYS lays;
        	lays.LayerCount = size;
        	lays.Layers = ReadFixedArray(data, ref offset, lays.LayerCount, ReadLayer);
        	return lays;
        }
    }
}