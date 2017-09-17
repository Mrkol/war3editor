using OpenTK;

namespace Editor.ModelRepresentation.Tracks
{
    public interface ITrack { }

    public class Track<T> : ITrack where T : struct
    {
        public int Frame;
        public T Value;
        public T? InTan;
        public T? OutTan;
    }

    public class TracksChunk<T> where T : ITrack
    {
        //remember: first come a 4 byte tag
        public uint TracksCount;
        public uint InterpolationType;
        public uint GlobalSequenceId;
        public T[] Tracks;
    } 

    //Node
    public class KGTR : Track<Vector3> { }
    public class KGRT : Track<Vector4> { }
    public class KGSC : Track<Vector3> { }

    //Layer
    public class KMTF : Track<uint> { }
    public class KMTA : Track<float> { }

    //Texture animation
    public class KTAT : Track<Vector3> { }
    public class KTAR : Track<Vector4> { }
    public class KTAS : Track<Vector3> { }

    //Geoset animation
    public class KGAO : Track<float> { }
    public class KGAC : Track<Vector3> { }

    //Light
    public class KLAS : Track<uint> { }
    public class KLAE : Track<uint> { }
    public class KLAC : Track<Vector3> { }
    public class KLAI : Track<float> { }
    public class KLBI : Track<float> { }
    public class KLBC : Track<Vector3> { }
    public class KLAV : Track<float> { }

    //Attachment
    public class KATV : Track<float> { }

    //Particle emitter
    public class KPEE : Track<float> { }
    public class KPEG : Track<float> { }
    public class KPLN : Track<float> { }
    public class KPLT : Track<float> { }
    public class KPEL : Track<float> { }
    public class KPES : Track<float> { }
    public class KPEV : Track<float> { }

    // Particle emitter 2
    public class KP2E : Track<float> { }
    public class KP2G : Track<float> { }
    public class KP2L : Track<float> { }
    public class KP2S : Track<float> { }
    public class KP2V : Track<float> { }
    public class KP2R : Track<float> { }
    public class KP2N : Track<float> { }
    public class KP2W : Track<float> { }

    // Ribbon emitter
    public class KRVS : Track<float> { }
    public class KRHA : Track<float> { }
    public class KRHB : Track<float> { }
    public class KRAL : Track<float> { }
    public class KRCO : Track<Vector3> { }
    public class KRTX : Track<uint> { }

    // Camera
    public class KCTR : Track<Vector3> { }
    public class KCRL : Track<uint> { }
    public class KTTR : Track<Vector3> { }
}
