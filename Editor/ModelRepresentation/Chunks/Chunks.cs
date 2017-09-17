using System.Runtime.InteropServices;
using Editor.ModelRepresentation.Objects;
using OpenTK;


namespace Editor.ModelRepresentation.Chunks
{
    /// <summary>
    /// Version chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VERS
    {
        public uint Version;
    }

    /// <summary>
    /// ModelX chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MODL
    {
        public string Name;
        public string AnimationFileName;
        public Extent Extent;
        public uint BlendTime;
    }

    /// <summary>
    /// Sequences chunk representation;
    /// </summary>
    public struct SEQS
    {
        public Sequence[] Sequences;
    }

    /// <summary>
    /// Global sequences chunk representation.
    /// </summary>
    public struct GLBS
    {
        public GlobalSequence[] Sequences;
    }

    /// <summary>
    /// Textures chunk representation.
    /// </summary>
    public struct TEXS
    {
        public Texture[] Textures;
    }

    /// <summary>
    /// Sound tracks chunk representation.
    /// </summary>
    public struct SNDS
    {
        public SoundTrack[] Tracks;
    }

    /// <summary>
    /// Materials chunk representation.
    /// </summary>
    public struct MTLS
    {
        public Material[] Materials;
    }

    /// <summary>
    /// Texture animations chunk representation.
    /// </summary>
    public struct TXAN
    {
        public TextureAnimation[] Animations;
    }

    /// <summary>
    /// Geosets chunk representation.
    /// </summary>
    public struct GEOS
    {
        public Geoset[] Geosets;
    }

    /// <summary>
    /// Geoset animations chunk representation.
    /// </summary>
    public struct GEOA
    {
        public GeosetAnimation[] Animations;
    }

    /// <summary>
    /// Bones chunk representation.
    /// </summary>
    public struct BONE
    {
        public Bone[] Bones;
    }

    /// <summary>
    /// Lights chunk representation.
    /// </summary>
    public struct LITE
    {
        public Light[] Lights;
    }

    /// <summary>
    /// Helpers chunk representation.
    /// </summary>
    public struct HELP
    {
        public Helper[] Helpers;
    }

    /// <summary>
    /// Attachments chunk representation.
    /// </summary>
    public struct ATCH
    {
        public Attachment[] Attachments;
    }

    /// <summary>
    /// Pivots chunk representation.
    /// </summary>
    public struct PIVT
    {
        public Vector3[] Points;
    }

    /// <summary>
    /// Particle emitters chunk representation.
    /// </summary>
    public struct PREM
    {
        public ParticleEmitter[] Emitters;
    }

    /// <summary>
    /// Particle emitters (var. 2) chunk representation.
    /// </summary>
    public struct PRE2
    {
        public ParticleEmitter2[] Emitters;
    }

    /// <summary>
    /// Ribbon emitters chunk representation.
    /// </summary>
    public struct RIBB
    {
        public RibbonEmitter[] Emitters;
    }

    /// <summary>
    /// Event objects chunk representation.
    /// </summary>
    public struct EVTS
    {
        public EventObject[] Objects;
    }

    /// <summary>
    /// Cameras chunk representation.
    /// </summary>
    public struct CAMS
    {
        public Camera[] Cameras;
    }

    /// <summary>
    /// Collision shapes chunk representation.
    /// </summary>
    public struct CLID
    {
        public CollisionShape[] Shapes;
    }

    /// <summary>
    /// Vertices chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VRTX
    {
        public uint Count;
        public Vector3[] Vertices;
    }

    /// <summary>
    /// Normals chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NRMS
    {
        public int Count;
        public Vector3[] Normals;
    }

    /// <summary>
    /// Texture coordinate set chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UVBS
    {
        public uint Count;
        public Vector2[] TextureCoordinates;
    }

    /// <summary>
    /// Texture coordinate sets chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UVAS
    {
        public uint TextureCoordinateSetsCount;
        public UVBS[] TextureCoordinateSets;
    }

    /// <summary>
    /// Primitive types chunk representation.
    /// Stores primitive type for each face group.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PTYP
    {
        public uint FaceGroupsCount;
        public uint[] FaceGroupPrimitiveTypes;
    }

    /// <summary>
    /// Primitive counts chunk representation.
    /// Stores primitive counts for each face group.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PCNT
    {
        public uint FaceGroupsCount;
        public uint FaceGroupPrimitiveCounts;
    }

    /// <summary>
    /// Face groups chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PVTX
    {
        public uint FaceGroupsCount;
        public ushort[] FaceGroups;
    }

    /// <summary>
    /// Vertex groups chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GNDX
    {
        public uint VertexGroupsCount;
        public byte[] VertexGroups;
    }

    /// <summary>
    /// Matrix groups chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MTGC
    {
        public uint MatrixGroupsCount;
        public uint[] MatrixGroups;
    }

    /// <summary>
    /// Matrices chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MATS
    {
        public uint MatrixIndicesCount;
        public uint[] MatrixIndices;
    }

    /// <summary>
    /// ???
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KEVT
    {
        public uint TracksCount;
        public uint GlobalSequenceId;
        public uint[] Tracks;
    }

    /// <summary>
    /// Layers chunk representation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LAYS
    {
        public uint LayerCount;
        public Layer[] Layers;
    }
}
