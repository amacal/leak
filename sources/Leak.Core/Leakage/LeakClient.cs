using Leak.Completion;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Listener;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Core.Spartan;
using System;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Core.Leakage
{
    public class LeakClient : IDisposable
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

            glue = new GlueFactory(new BufferedBlockFactory());
        }

        public void Register(LeakRegistrant registrant)
        {
            LeakEntry entry = collections.Register(registrant);

            entry.Destination = registrant.Destination;
            entry.Glue = glue.Create(entry.Hash, CreateGlueHooks(), CreateGlueConfiguration(entry));

            entry.Spartan = new SpartanService(null, entry.Destination, entry.Glue, null, CreateSpartanHooks(), CreateSpartanConfiguration());
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

        private GlueConfiguration CreateGlueConfiguration(LeakEntry entry)
        {
            return new GlueConfiguration
            {
                Plugins = new List<GluePlugin>
                {
                    new MetadataPlugin(CreateMetadataHooks(entry))
                }
            };
        }

        private MetadataHooks CreateMetadataHooks(LeakEntry entry)
        {
            return new MetadataHooks
            {
                OnMetadataMeasured = entry.Spartan.HandleMetadataMeasured,
                OnMetadataReceived = entry.Spartan.HandleMetadataReceived
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

        public void Dispose()
        {
        }
    }
}