using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Telegraph;
using Leak.Core.Tracker;

namespace Leak.Core.Client
{
    public class PeerClientTelegraphConnect : TrackerTelegraphCallbackBase
    {
        private readonly PeerClientCallback callback;
        private readonly FileHash hash;
        private readonly PeerConnector connector;
        private readonly PeerClientStorage storage;

        public PeerClientTelegraphConnect(PeerClientConfiguration configuration, FileHash hash, PeerConnector connector, PeerClientStorage storage)
        {
            this.callback = configuration.Callback;

            this.hash = hash;
            this.connector = connector;
            this.storage = storage;
        }

        public override void OnAnnounced(TrackerAnnounce announce)
        {
            foreach (TrackerPeer peer in announce.Peers)
            {
                if (storage.Contains(peer.Host) == false)
                {
                    callback.OnPeerConnecting(hash, $"{peer.Host}:{peer.Port}");
                    connector.ConnectTo(peer.Host, peer.Port);
                }
            }
        }
    }
}