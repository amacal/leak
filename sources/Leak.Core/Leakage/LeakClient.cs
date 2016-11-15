using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Listener;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Core.Spartan;

namespace Leak.Core.Leakage
{
    public class LeakClient
    {
        private readonly NetworkPool network;
        private readonly PeerListener listener;
        private readonly GlueFactory glue;

        private readonly CompletionThread worker;
        private readonly LeakCollection collections;

        public LeakClient()
        {
            collections = new LeakCollection();
            worker = new CompletionThread();

            network = new NetworkPool(worker, CreateNetworkHooks());
            listener = new PeerListener(network, CreateListenerHooks(), CreateListenerConfiguration());
        }

        public void Register(LeakRegistrant registrant)
        {
            LeakEntry entry = collections.Register(registrant);

            entry.Destination = registrant.Destination;
            entry.Glue = glue.Create(entry.Hash, CreateGlueHooks());

            entry.Spartan = new SpartanService(entry.Hash, entry.Destination, entry.Glue, CreateSpartanHooks(), CreateSpartanConfiguration());
            entry.Spartan.Start();

            listener.Enable(entry.Hash);
        }

        private NetworkPoolHooks CreateNetworkHooks()
        {
            return new NetworkPoolHooks
            {
            };
        }

        private PeerListenerHooks CreateListenerHooks()
        {
            return new PeerListenerHooks
            {
                OnHandshakeCompleted = OnHandshakeCompleted
            };
        }

        private PeerListenerConfiguration CreateListenerConfiguration()
        {
            return new PeerListenerConfiguration
            {
            };
        }

        private GlueHooks CreateGlueHooks()
        {
            return new GlueHooks
            {
            };
        }

        private SpartanHooks CreateSpartanHooks()
        {
            return new SpartanHooks
            {
            };
        }

        private SpartanConfiguration CreateSpartanConfiguration()
        {
            return new SpartanConfiguration
            {
            };
        }

        private void OnHandshakeCompleted(HandshakeCompleted data)
        {
            FileHash hash = data.Handshake.Hash;
            LeakEntry entry = collections.Find(hash);

            if (entry != null)
            {
                Handshake handshake = data.Handshake;
                NetworkConnection connection = data.Connection;

                entry.Glue.Connect(connection, handshake);
            }
        }
    }
}