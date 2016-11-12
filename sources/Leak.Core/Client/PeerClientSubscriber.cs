using Leak.Core.Client.Events;
using Leak.Core.Common;

namespace Leak.Core.Client
{
    public class PeerClientSubscriber
    {
        private readonly PeerClientContext context;

        public PeerClientSubscriber(PeerClientContext context)
        {
            this.context = context;
        }

        public void Handle(string name, dynamic payload)
        {
            switch (name)
            {
                case "tracker-announce-completed":
                    HandleTrackerAnnounceCompleted(payload);
                    break;

                case "metadata-size-received":
                    HandleMetadataSizeReceived(payload);
                    break;
            }
        }

        private void HandleTrackerAnnounceCompleted(dynamic payload)
        {
            context.Bus.Publish("file-announced", new FileAnnounced
            {
                Hash = payload.Hash,
                Peer = payload.Peer,
                Count = payload.Peers.Length
            });

            if (context.Connector != null)
            {
                foreach (PeerAddress peer in payload.Peers)
                {
                    context.Connector.ConnectTo(payload.Hash, peer);
                }
            }
        }

        private void HandleMetadataSizeReceived(dynamic payload)
        {
            context.Scheduler.Handle(with =>
            {
                with.OnMetadataSize(payload.Peer, payload.Size);
            });
        }
    }
}