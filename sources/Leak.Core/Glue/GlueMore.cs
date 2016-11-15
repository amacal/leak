using System.Collections.Generic;

namespace Leak.Core.Glue
{
    public class GlueMore
    {
        private readonly Dictionary<byte, string> byId;
        private readonly Dictionary<string, byte> byCode;

        public GlueMore()
        {
            byId = new Dictionary<byte, string>();
            byCode = new Dictionary<string, byte>();
        }

        public void Add(string name, GlueHandler handler)
        {
        }

        public void Register(byte id, string name)
        {
        }
    }
}