using Leak.Common;
using Leak.Extensions;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorNamedPlugin : MorePlugin, MoreHandler
    {
        private readonly string name;

        public CoordinatorNamedPlugin(string name)
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