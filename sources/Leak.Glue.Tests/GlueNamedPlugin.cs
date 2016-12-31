using Leak.Common;
using Leak.Extensions;

namespace Leak.Glue.Tests
{
    public class GlueNamedPlugin : MorePlugin, MoreHandler
    {
        private readonly string name;

        public GlueNamedPlugin(string name)
        {
            this.name = name;
        }

        public void Install(MoreMapping mapping)
        {
            mapping.Add(name, this);
        }

        public void OnMessageReceived(FileHash hash, PeerHash peer, byte[] payload)
        {
        }

        public void OnMessageSent(FileHash hash, PeerHash peer, byte[] payload)
        {
        }

        public void OnHandshake(FileHash hash, PeerHash peer, byte[] payload)
        {
        }
    }
}
