using Leak.Core.Common;
using Leak.Core.Extensions;

namespace Leak.Core.Client
{
    public class PeerClientToExtender : ExtenderCallbackBase
    {
        private readonly FileHash hash;
        private readonly PeerClientStorage storage;

        public PeerClientToExtender(FileHash hash, PeerClientStorage storage)
        {
            this.hash = hash;
            this.storage = storage;
        }

        public override void OnHandshake(PeerHash peer, ExtenderHandshake handshake)
        {
            storage.GetRetriever(peer).SetExtensions(peer, handshake);
        }
    }
}