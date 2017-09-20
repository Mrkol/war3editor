using Editor.ModelRepresentation.Chunks;
using OpenTK;

namespace Editor.ModelRepresentation
{
    static partial class Parser
    {
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

        public static TEXS ReadTEXS(byte[] data, ref int offset, int size)
        {
            TEXS texs;
            texs.Textures = ReadArray(data, ref offset, size, ReadTexture);
            return texs;
        }

        public static GEOS ReadGEOS(byte[] data, ref int offset, int size)
        {
            GEOS geos;
            geos.Geosets = ReadArray(data, ref offset, size, ReadGeoset);
            return geos;
        }

        public static VRTX ReadVRTX(byte[] data, ref int offset, int size)
        {
            VRTX vrtx;
            vrtx.VerticesCount = ReadUint(data, ref offset);
            vrtx.Vertices = ReadFixedArray<Vector3>(data, ref offset, vrtx.VerticesCount, ReadStruct<Vector3>);
            return vrtx;
        }

        public static NRMS ReadNRMS(byte[] data, ref int offset, int size)
        {
            NRMS nrms;
            nrms.NormalsCount = ReadUint(data, ref offset);
            nrms.Normals = ReadFixedArray<Vector3>(data, ref offset, nrms.NormalsCount, ReadStruct<Vector3>);
            return nrms;
        }

        public static PTYP ReadPTYP(byte[] data, ref int offset, int size)
        {
            PTYP ptyp;
            ptyp.FaceGroupsCount = ReadUint(data, ref offset);
            ptyp.FaceGroupPrimitiveTypes = ReadFixedArray<uint>(data, ref offset, ptyp.FaceGroupsCount, ReadUint);
            return ptyp;
        }

        public static PCNT ReadPCNT(byte[] data, ref int offset, int size)
        {
            PCNT pcnt;
            pcnt.FaceGroupsCount = ReadUint(data, ref offset);
            pcnt.FaceGroupPrimitiveCounts = ReadFixedArray(data, ref offset, pcnt.FaceGroupsCount, ReadUint);
            return pcnt;
        }

        public static PVTX ReadPVTX(byte[] data, ref int offset, int size)
        {
            PVTX pvtx;
            pvtx.FaceGroupsCount = ReadUint(data, ref offset);
            pvtx.FaceGroups = ReadFixedArray(data, ref offset, pvtx.FaceGroupsCount, ReadUshort);
            return pvtx;
        }

        public static GNDX ReadGNDX(byte[] data, ref int offset, int size)
        {
            GNDX gndx;
            gndx.VertexGroupsCount = ReadUint(data, ref offset);
            gndx.VertexGroups = ReadFixedArray(data, ref offset, gndx.VertexGroupsCount, ReadByte);
            return gndx;
        }

        public static MTGC ReadMTGC(byte[] data, ref int offset, int size)
        {
            MTGC mtgc;
            mtgc.MatrixGroupsCount = ReadUint(data, ref offset);
            mtgc.MatrixGroups = ReadFixedArray(data, ref offset, mtgc.MatrixGroupsCount, ReadUint);
            return mtgc;
        }

        public static MATS ReadMATS(byte[] data, ref int offset, int size)
        {
            MATS mats;
            mats.MatrixIndicesCount = ReadUint(data, ref offset);
            mats.MatrixIndices = ReadFixedArray(data, ref offset, mats.MatrixIndicesCount, ReadUint);
            return mats;
        }

        public static UVAS ReadUVAS(byte[] data, ref int offset, int size)
        {
            UVAS uvas;
            uvas.TextureCoordinateSetsCount = ReadUint(data, ref offset);
            uvas.TextureCoordinateSets = ReadFixedArray(data, ref offset, uvas.TextureCoordinateSetsCount, 
                    (byte[] d, ref int o) => ReadChunk<UVBS>(d, ref o, ReadUVBS).Value);
            return uvas;
        }

        public static UVBS ReadUVBS(byte[] data, ref int offset, int size)
        {
            UVBS uvbs;
            uvbs.TextureCoordinatesCount = ReadUint(data, ref offset);
            uvbs.TextureCoordinates = ReadFixedArray(data, ref offset, uvbs.TextureCoordinatesCount, ReadStruct<Vector2>);
            return uvbs;
        }
    }
}