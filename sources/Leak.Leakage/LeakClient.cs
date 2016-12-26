using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Completion;
using Leak.Connector;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Files;
using Leak.Glue;
using Leak.Listener;
using Leak.Memory;
using Leak.Negotiator;
using Leak.Networking;
using Leak.Spartan;
using Leak.Tasks;

namespace Leak.Leakage
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

        private readonly HandshakeNegotiator negotiator;
        private readonly HandshakeNegotiatorPassiveContext negotiatorContext;

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

            network = new NetworkPoolFactory(pipeline, worker).CreateInstance(CreateNetworkHooks());
            network.Start();

            listener = CreateListener();
            glue = new GlueFactory(new BufferedBlockFactory());

            negotiatorContext = new HandshakeNegotiatorPassiveInstance(configuration.Peer);
            negotiator = CreateNegotiator();
        }

        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        private PeerListener CreateListener()
        {
            PeerListener instance = null;

            if (this.configuration.Port != LeakPort.Nothing)
            {
                PeerListenerHooks listenerHooks = CreateListenerHooks();
                PeerListenerConfiguration listenerConfiguration = CreateListenerConfiguration();

                instance = new PeerListener(network, listenerHooks, listenerConfiguration);
            }

            return instance;
        }

        private HandshakeNegotiator CreateNegotiator()
        {
            HandshakeNegotiatorFactory factory = new HandshakeNegotiatorFactory(network);
            HandshakeNegotiatorHooks handshakeHooks = new HandshakeNegotiatorHooks
            {
                OnHandshakeCompleted = OnHandshakeCompleted
            };

            return factory.Create(handshakeHooks);
        }

        public void Start()
        {
            listener?.Start();
        }

        public void Register(LeakRegistrant registrant)
        {
            LeakEntry entry = collections.Register(registrant);

            entry.Destination = registrant.Destination;
            entry.MetadataHooks = new MetadataHooks();

            entry.NegotiatorHooks = CreateNegotiatorHooks(entry);
            entry.Negotiator = new HandshakeNegotiatorFactory(network).Create(entry.NegotiatorHooks);

            entry.GlueHooks = new GlueHooks();
            entry.Glue = glue.Create(entry.Hash, entry.GlueHooks, CreateGlueConfiguration(entry));

            entry.SpartaHooks = new SpartanHooks();
            entry.Spartan = new SpartanService(pipeline, entry.Destination, entry.Glue, files, CreateSpartanHooks(), CreateSpartanConfiguration());

            entry.ConnectorHooks = CreateConnectorHooks(entry);
            entry.Connector = new PeerConnector(network, entry.ConnectorHooks, new PeerConnectorConfiguration());

            AttachHooks(entry);

            entry.Spartan.Start();
            entry.Connector.Start(pipeline);

            negotiatorContext.Hashes.Add(entry.Hash);

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
            entry.MetadataHooks.OnMetadataPieceSent = entry.Spartan.HandleMetadataReceived;

            //entry.ConnectorHooks.OnHandshakeCompleted = OnHandshakeCompleted;
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
                OnListenerStarted = hooks.CallListenerStarted,
                OnConnectionArrived = OnConnectionArrived
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

        private HandshakeNegotiatorHooks CreateNegotiatorHooks(LeakEntry entry)
        {
            return new HandshakeNegotiatorHooks
            {
                OnHandshakeCompleted = OnHandshakeCompleted
            };
        }

        private PeerConnectorHooks CreateConnectorHooks(LeakEntry entry)
        {
            return new PeerConnectorHooks
            {
                OnConnectionEstablished = data => OnConnectionEstablished(data, entry)
            };
        }

        private void OnConnectionArrived(ConnectionArrived data)
        {
            negotiator.Handle(data.Connection, negotiatorContext);
        }

        private void OnConnectionEstablished(ConnectionEstablished data, LeakEntry entry)
        {
            entry.Negotiator.Start(data.Connection, entry.Hash);
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