using Leak.Common;
using Leak.Completion;
using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Listener;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Core.Spartan;
using System;
using System.Collections.Generic;

namespace Leak.Core.Leakage
{
    public class LeakClient : IDisposable
    {
        private readonly LeakHooks hooks;
        private readonly LeakConfiguration configuration;

        private readonly NetworkPool network;
        private readonly PeerListener listener;
        private readonly GlueFactory glue;

        private readonly CompletionThread worker;
        private readonly LeakCollection collections;
        private readonly LeakPipeline pipeline;

        public LeakClient(LeakHooks hooks, LeakConfiguration configuration)
        {
            this.hooks = hooks;
            this.configuration = configuration;

            collections = new LeakCollection();
            worker = new CompletionThread();
            pipeline = new LeakPipeline();

            network = new NetworkPool(pipeline, worker, CreateNetworkHooks());
            listener = CreateAndStartListener();

            glue = new GlueFactory(new BufferedBlockFactory());

            pipeline.Start();
            worker.Start();
            network.Start();
        }

        private PeerListener CreateAndStartListener()
        {
            PeerListener instance = null;

            if (this.configuration.Port.HasValue)
            {
                PeerListenerHooks hooks = CreateListenerHooks();
                PeerListenerConfiguration configuration = CreateListenerConfiguration();

                instance = new PeerListener(network, hooks, configuration);
                instance.Start();
            }

            return instance;
        }

        public void Register(LeakRegistrant registrant)
        {
            LeakEntry entry = collections.Register(registrant);

            entry.Destination = registrant.Destination;
            entry.Glue = glue.Create(entry.Hash, CreateGlueHooks(entry), CreateGlueConfiguration(entry));

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
                OnListenerStarted = hooks.OnListenerStarted,
                OnHandshakeCompleted = OnHandshakeCompleted
            };
        }

        private PeerListenerConfiguration CreateListenerConfiguration()
        {
            return new PeerListenerConfiguration
            {
                Port = configuration.Port.Value,
                Peer = configuration.Peer,
                Extensions = true
            };
        }

        private GlueHooks CreateGlueHooks(LeakEntry entry)
        {
            return new GlueHooks
            {
                OnPeerChanged = entry.Spartan.HandlePeerChanged,
                OnBlockReceived = entry.Spartan.HandleBlockReceived
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
            pipeline.Stop();
            listener?.Stop();
            worker.Dispose();
        }
    }
}