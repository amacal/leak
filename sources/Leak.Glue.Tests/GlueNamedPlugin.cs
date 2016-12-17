using Leak.Common;

namespace Leak.Glue.Tests
{
    public class GlueNamedPlugin : GluePlugin, GlueHandler
    {
        private readonly string name;

        public GlueNamedPlugin(string name)
        {
            this.name = name;
        }

        public void Install(GlueMore more)
        {
            more.Add(name, this);
        }

        public void OnMessageReceived(FileHash hash, PeerHash peer, byte[] payload)
        {
        }

        public void OnMessageSent(FileHash hash, PeerHash peer, byte[] payload)
        {
        }
    }
}
