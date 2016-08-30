using Leak.Core.Client.Events;
using Leak.Core.Common;
using Leak.Core.Telegraph;
using Leak.Core.Tracker;

namespace Leak.Core.Client.Callbacks
{
    public class PeerClientToTelegraph : TrackerTelegraphCallbackBase
    {
        private readonly PeerClientContext context;

        public PeerClientToTelegraph(PeerClientContext context)
        {
            this.context = context;
        }

        public override void OnAnnouncingCompleted(TrackerAnnounce announce)
        {
            context.Callback.OnAnnounceCompleted(announce.Hash, new FileAnnouncedEvent(announce));

            foreach (PeerAddress peer in announce.Peers)
            {
                context.Connector.ConnectTo(announce.Hash, peer);
            }
        }
    }
}