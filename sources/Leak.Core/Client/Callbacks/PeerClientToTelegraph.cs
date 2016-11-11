using Leak.Core.Client.Events;
using Leak.Core.Common;
using Leak.Core.Telegraph;
using Leak.Core.Tracker;

namespace Leak.Core.Client.Callbacks
{
    public class PeerClientToTelegraph : TelegraphCallbackBase
    {
        private readonly PeerClientContext context;

        public PeerClientToTelegraph(PeerClientContext context)
        {
            this.context = context;
        }

        public override void OnAnnouncingCompleted(TrackerAnnounce announce)
        {
            context.Bus.Publish("file-announced", new FileAnnounced
            {
                Hash = announce.Hash,
                Peer = announce.Peer,
                Count = announce.Peers.Length
            });

            if (context.Connector != null)
            {
                foreach (PeerAddress peer in announce.Peers)
                {
                    context.Connector.ConnectTo(announce.Hash, peer);
                }
            }
        }
    }
}