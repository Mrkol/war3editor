using System.IO;
using System.Runtime.CompilerServices;
using MpqLib;

namespace Editor.MapRepresentation
{
    class Map
    {
        public Environment Environment { get; private set; }

        public Map()
        {
            Environment = null;
        }

        public static Map ImportMpq(Mpq.FileDescriptor mpq)
        {
            Map m = new Map();
            
            MemoryStream envStream = new MemoryStream();
            Mpq.ExtractFile(mpq, "war3map.w3e", envStream);
            m.Environment = Environment.Read(envStream.ToArray());
            
            return m;
        }
    }
}
