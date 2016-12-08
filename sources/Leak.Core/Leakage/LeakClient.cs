using Leak.Common;
using Leak.Completion;
using Leak.Core.Connector;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Listener;
using Leak.Core.Spartan;
using Leak.Events;
using Leak.Files;
using Leak.Networking;
using Leak.Tasks;
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
        private readonly FileFactory files;

        public LeakClient(LeakHooks hooks, LeakConfiguration configuration)
        {
            this.hooks = hooks;
            this.configuration = configuration;

            collections = new LeakCollection();
            worker = new CompletionThread();
            pipeline = new LeakPipeline();
            files = new FileFactory(worker);

            pipeline.Start();
            worker.Start();

            network = new NetworkPool(pipeline, worker, CreateNetworkHooks());
            network.Start();

            listener = CreateAndStartListener();
            glue = new GlueFactory(new BufferedBlockFactory());
        }

        private PeerListener CreateAndStartListener()
        {
            PeerListener instance = null;

            if (this.configuration.Port != LeakPort.Nothing)
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
            entry.MetadataHooks = new MetadataHooks();

            entry.GlueHooks = new GlueHooks();
            entry.Glue = glue.Create(entry.Hash, entry.GlueHooks, CreateGlueConfiguration(entry));

            entry.SpartaHooks = new SpartanHooks();
            entry.Spartan = new SpartanService(pipeline, entry.Destination, entry.Glue, files, CreateSpartanHooks(), CreateSpartanConfiguration());

            entry.ConnectorHooks = new PeerConnectorHooks();
            entry.Connector = new PeerConnector(network, entry.ConnectorHooks, new PeerConnectorConfiguration());

            AttachHooks(entry);

            entry.Spartan.Start();
            entry.Connector.Start(pipeline);

            listener?.Enable(entry.Hash);

            foreach (PeerAddress peer in registrant.Peers)
            {
                entry.Connector.ConnectTo(registrant.Hash, peer);
            }
        }

        private void AttachHooks(LeakEntry entry)
        {
            entry.GlueHooks.OnPeerChanged = entry.Spartan.HandlePeerChanged;
            entry.GlueHooks.OnBlockReceived = entry.Spartan.HandleBlockReceived;
            entry.GlueHooks.OnPeerConnected = hooks.OnPeerConnected;

            entry.MetadataHooks.OnMetadataMeasured = entry.Spartan.HandleMetadataMeasured;
            entry.MetadataHooks.OnMetadataReceived = entry.Spartan.HandleMetadataReceived;

            entry.ConnectorHooks.OnHandshakeCompleted = OnHandshakeCompleted;
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
            PeerListenerPort port = new PeerListenerPortRandom();

            if (configuration.Port != LeakPort.Random)
            {
                port = new PeerListenerPortValue(configuration.Port.Value);
            }

            return new PeerListenerConfiguration
            {
                Port = port,
                Peer = configuration.Peer,
                Extensions = true
            };
        }

        private GlueConfiguration CreateGlueConfiguration(LeakEntry entry)
        {
            return new GlueConfiguration
            {
                Plugins = new List<GluePlugin>
                {
                    new MetadataPlugin(entry.MetadataHooks)
                }
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