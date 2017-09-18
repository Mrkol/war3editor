using System.Runtime.InteropServices;
using Editor.ModelRepresentation.Chunks;
using Editor.ModelRepresentation.Tracks;
using OpenTK;


//Classes in this namespace are all unified by the fact that they don't have a char[4] prefix in the format
namespace Editor.ModelRepresentation.Objects
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Extent
    {
        public float BoundsRadius;
        public Vector3 Minimum;
        public Vector3 Maximum;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Node
    {
        public uint InclusiveSize;
        public string Name;
        public uint ObjectId;
        public uint ParentId;
        public uint Flags;
        public TracksChunk<KGTR> Kgtr;
        public TracksChunk<KGRT> Kgrt;
        public TracksChunk<KGSC> Kgsc;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Sequence
    {
        public string Name;
        public fixed uint Interval[2];
        public float MoveSpeed;
        public uint Flags;
        public float Rarity;
        public uint SyncPoint;
        public Extent Extent;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GlobalSequence
    {
        public uint Duration;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Texture
    {
        public uint ReplaceableId;
        public string FileName;
        public uint Flags;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SoundTrack
    {
        public string FileName;
        public float Volume;
        public float Pitch;
        public uint Flags;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Material
    {
        public uint InclusiveSize;
        public uint PriorityPlane;
        public uint Flags;
        public LAYS Lays;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Layer
    {
        public uint InclusiveSize;
        public uint FilterMode;
        public uint ShadingFlags;
        public uint TextureId;
        public uint TextureAnimationId;
        public uint CoordId;
        public float Alpha;
        public TracksChunk<KMTF> Kmtf;
        public TracksChunk<KMTA> Kmta;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextureAnimation
    {
        public uint InclusiveSize;
        public TracksChunk<KTAT> Ktat;
        public TracksChunk<KTAR> Ktar;
        public TracksChunk<KTAS> Ktas;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Geoset
    {
        public uint InclusiveSize;
        public VRTX Vrtx;
        public NRMS Nrms;
        public PTYP Ptyp;
        public PCNT Pcnt;
        public PVTX Pvtx;
        public GNDX Gndx;
        public MTGC Mtgc;
        public MATS Mats;
        public uint MaterialId;
        public uint SelectionGroup;
        public uint SelectionFlags;
        public Extent Extent;
        public uint ExtentsCount;
        public Extent[] Extents;
        public UVAS Uvas;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GeosetAnimation
    {
        public uint InclusiveSize;
        public float Alpha;
        public uint Flags;
        public Vector3 Color;
        public uint GeosetId;
        public TracksChunk<KGAO> Kgao;
        public TracksChunk<KGAC> Kgac;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bone
    {
        public Node Node;
        public uint GeosetId;
        public uint GeosetAnimationId;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Light
    {
        public uint InclusiveSize;
        public Node Node;
        public uint Type;
        public float AttenuationStart;
        public float AttenuationEnd;
        public Vector3 Color;
        public float Intensity;
        public Vector3 AmbientColor;
        public float AmbientIntensity;
        public TracksChunk<KLAS> Klas;
        public TracksChunk<KLAE> Klae;
        public TracksChunk<KLAC> Klac;
        public TracksChunk<KLAI> Klai;
        public TracksChunk<KLBI> Klbi;
        public TracksChunk<KLBC> Klbc;
        public TracksChunk<KLAV> Klav;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Helper
    {
        public Node Node;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Attachment
    {
        public uint InclusiveSize;
        public Node Node;
        public string Path;
        public uint AttachmentId;
        public TracksChunk<KATV> Katv;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticleEmitter
    {
        public uint InclusiveSize;
        public Node Node;
        public float EmissionRate;
        public float Gravity;
        public float Longtitude;
        public float Latitude;
        public string SpawnModelFileName;
        public float Lifespan;
        public float InitialVelocity;
        public TracksChunk<KPEE> Kpee;
        public TracksChunk<KPEG> Kpeg;
        public TracksChunk<KPLN> Kpln;
        public TracksChunk<KPLT> Kplt;
        public TracksChunk<KPEL> Kpel;
        public TracksChunk<KPES> Kpes;
        public TracksChunk<KPEV> Kpev;


    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ParticleEmitter2
    {
        public uint InclusiveSize;
        public Node Node;
        public float Speed;
        public float Variation;
        public float Latitude;
        public float Gravity;
        public float Lifespan;
        public float EmissionRate;
        public float Width;
        public float Length;
        public uint FilterMode;
        public uint Rows;
        public uint Columns;
        public uint HeadOrTail;
        public float TailLength;
        public float Time;
        public Matrix3 SegmentColor;
        public fixed byte SegmentAlpha[3];
        public fixed float SegmentScaling[3];
        public fixed uint HeadInterval[3];
        public fixed uint HeadDecayInterval[3];
        public fixed uint TailInterval[3];
        public fixed uint TailDecayInterval[3];
        public uint TextureId;
        public uint Squirt;
        public uint PriorityPlane;
        public uint ReplaceableId;
        public TracksChunk<KP2S> Kp2s;
        public TracksChunk<KP2R> Kp2r;
        public TracksChunk<KP2L> Kp2l;
        public TracksChunk<KP2G> Kp2g;
        public TracksChunk<KP2E> Kp2e;
        public TracksChunk<KP2N> Kp2n;
        public TracksChunk<KP2W> Kp2w;
        public TracksChunk<KP2V> Kp2v;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RibbonEmitter
    {
        public uint InclusiveSize;
        public Node node;
        public float HeightAbove;
        public float HeightBelow;
        public float Alpha;
        public Vector3 Color;
        public float Lifespan;
        public uint TextureSlot;
        public uint EmissionRate;
        public uint Rows;
        public uint Columns;
        public uint MaterialId;
        public float Gravity;
        public TracksChunk<KRHA> Krha;
        public TracksChunk<KRHB> Krhb;
        public TracksChunk<KRAL> Kral;
        public TracksChunk<KRCO> Krco;
        public TracksChunk<KRTX> Krtx;
        public TracksChunk<KRVS> Krvs;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EventObject
    {
        public Node Node;
        public KEVT Kevt;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Camera
    {
        public uint InclusiveSize;
        public string Name;
        public Vector3 Position;
        public float FieldOfView;
        public float FarClippingPlane;
        public float NearClippingPlane;
        public Vector3 TargetPosition;
        public TracksChunk<KCTR> Kctr;
        public TracksChunk<KTTR> Kttr;
        public TracksChunk<KCRL> Kcrl;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CollisionShape
    {
        public Node Node;
        public uint Type;
        public Vector3[] Vertices;
        public float? Radius;
    }
}
