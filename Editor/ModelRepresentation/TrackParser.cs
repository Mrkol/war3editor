using Editor.ModelRepresentation.Tracks;

namespace Editor.ModelRepresentation
{
    static partial class Parser
    {
    	public static TracksChunk<TTrack> ReadTracksChunk<TTrack, TValue>(byte[] data, ref int offset) 
    		where TTrack : Track<TValue>, new()
    		where TValue : struct
    	{
            bool tagPresent = TryReadTag(data, ref offset, Chunk.FromType<TTrack>());
            if (!tagPresent) return null;
            TracksChunk<TTrack> tracks = new TracksChunk<TTrack>();
            tracks.TracksCount = ReadUint(data, ref offset);
            tracks.InterpolationType = ReadUint(data, ref offset);
            tracks.GlobalSequenceId = ReadUint(data, ref offset);

            tracks.Tracks = ReadFixedArray<TTrack>(data, ref offset, tracks.TracksCount, 
            	(byte[] d, ref int o) => ReadTrack<TTrack, TValue>(d, ref o, tracks.InterpolationType));

            return tracks;
    	}

    	public static TTrack ReadTrack<TTrack, TValue>(byte[] data, ref int offset, uint interpolationType) 
    		where TTrack : Track<TValue>, new()
    		where TValue : struct
    	{
    		TTrack track = new TTrack();
    		track.Frame = ReadInt(data, ref offset);
    		track.Value = ReadStruct<TValue>(data, ref offset);
    		if (interpolationType > 1)
    		{
    			track.InTan = ReadStruct<TValue>(data, ref offset);
    			track.OutTan = ReadStruct<TValue>(data, ref offset);
    		}
    		return track;
    	}
    }
}