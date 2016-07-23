using Leak.Core.Connector;
using Leak.Core.Metadata;
using Leak.Core.Telegraph;
using Leak.Core.Tracker;

namespace Leak.Core.Client
{
    public class PeerClientToTelegraph : TrackerTelegraphCallbackBase
    {
        private readonly PeerClientConfiguration configuration;
        private readonly PeerClientCallback callback;
        private readonly Metainfo metainfo;
        private readonly PeerConnector connector;
        private readonly PeerClientStorage storage;

        public PeerClientToTelegraph(PeerClientConfiguration configuration, Metainfo metainfo, PeerConnector connector, PeerClientStorage storage)
        {
            this.configuration = configuration;
            this.callback = configuration.Callback;

            this.metainfo = metainfo;
            this.connector = connector;
            this.storage = storage;
        }

        public override void OnAnnounced(TrackerAnnounce announce)
        {
            foreach (TrackerPeer peer in announce.Peers)
            {
                if (storage.Contains(peer.Host) == false)
                {
                    callback.OnPeerConnecting(metainfo, $"{peer.Host}:{peer.Port}");
                    connector.ConnectTo(peer.Host, peer.Port);
                }
            }
        }
    }
}