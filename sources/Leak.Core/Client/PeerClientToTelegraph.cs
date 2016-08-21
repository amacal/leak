using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Telegraph;
using Leak.Core.Tracker;

namespace Leak.Core.Client
{
    public class PeerClientToTelegraph : TrackerTelegraphCallbackBase
    {
        private readonly PeerClientContext context;

        public PeerClientToTelegraph(PeerClientContext context)
        {
            this.context = context;
        }

        public override void OnAnnounced(TrackerAnnounce announce)
        {
            PeerConnector connector = new PeerConnector(with =>
            {
                with.Peer = context.Peer;
                with.Extensions = true;
                with.Hash = announce.Hash;
                with.Pool = context.Network;
                with.Callback = context.Collector.CreateConnectorCallback();
            });

            foreach (PeerAddress peer in announce.Peers)
            {
                connector.ConnectTo(peer);
            }
        }
    }
}