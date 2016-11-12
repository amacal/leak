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
            bool accepted = false;

            lock (context.Synchronized)
            {
                if (context.Bouncer.AcceptRemote(connection))
                {
                    accepted = true;
                }
            }

            if (accepted == false)
            {
                connection.Terminate();
            }
        }
    }
}