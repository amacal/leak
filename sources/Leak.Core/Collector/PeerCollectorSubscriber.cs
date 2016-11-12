using Leak.Core.Collector.Events;
using Leak.Core.Common;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public class PeerCollectorSubscriber
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorSubscriber(PeerCollectorContext context)
        {
            this.context = context;
        }

        public void Handle(string name, dynamic payload)
        {
            switch (name)
            {
                case "listener-accepted":
                    HandleConnected(payload.Connection);
                    break;
            }
        }

        private void HandleConnected(NetworkConnection connection)
        {
            int total = 0;
            bool accepted = false;

            lock (context.Synchronized)
            {
                if (context.Bouncer.AcceptRemote(connection))
                {
                    accepted = true;
                    total = context.Bouncer.Count();
                }
            }

            if (accepted)
            {
                PeerAddress peer = connection.Remote;
                PeerCollectorConnected connected = new PeerCollectorConnected(peer, total);

                context.Callback.OnConnectedFrom(connected);
            }
            else
            {
                connection.Terminate();
            }
        }
    }
}