using System;
using System.IO;
using Leak.Common;
using Leak.Completion;
using Leak.Connector;
using Leak.Datashare;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Files;
using Leak.Glue;
using Leak.Listener;
using Leak.Memory;
using Leak.Metafile;
using Leak.Metaget;
using Leak.Metashare;
using Leak.Negotiator;
using Leak.Networking;
using Leak.Omnibus;
using Leak.Repository;
using Leak.Retriever;
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

            negotiatorContext = new HandshakeNegotiatorPassiveInstance(configuration.Peer);
            negotiator = new HandshakeNegotiatorBuilder().WithNetwork(network).Build();
            negotiator.Hooks.OnHandshakeCompleted += OnHandshakeCompleted;
        }

        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        private PeerListener CreateListener()
        {
            PeerListener instance = null;
            PeerListenerBuilder builder = null;

            if (configuration.Port != LeakPort.Nothing)
            {
                builder =
                    new PeerListenerBuilder()
                        .WithPeer(configuration.Peer)
                        .WithExtensions()
                        .WithNetwork(network);
            }

            if (configuration.Port != LeakPort.Random)
            {
                builder?.WithPort(configuration.Port.Value);
            }

            instance = builder?.Build();

            if (instance != null)
            {
                instance.Hooks.OnListenerStarted = hooks.CallListenerStarted;
                instance.Hooks.OnConnectionArrived = OnConnectionArrived;
            }

            return instance;
        }

        public void Start()
        {
            listener?.Start();
        }

        public void Register(LeakRegistrant registrant)
        {
            LeakEntry entry = collections.Register(registrant);

            entry.MetadataPlugin =
                new MetadataBuilder()
                    .Build();

            entry.PeersPlugin =
                new PeersBuilder()
                    .Build();

            entry.Negotiator =
                new HandshakeNegotiatorBuilder()
                    .WithNetwork(network)
                    .Build();

            entry.Glue =
                new GlueBuilder()
                    .WithHash(entry.Hash)
                    .WithBlocks(new BufferedBlockFactory())
                    .WithPlugin(entry.MetadataPlugin)
                    .WithPlugin(entry.PeersPlugin)
                    .Build();

            entry.Metafile =
                new MetafileBuilder()
                    .WithHash(entry.Hash)
                    .WithDestination(Path.Combine(entry.Destination, entry.Hash.ToString()) + ".metainfo")
                    .WithFiles(files)
                    .WithPipeline(pipeline)
                    .Build();

            entry.Metaget =
                new MetagetBuilder()
                    .WithHash(entry.Hash)
                    .WithPipeline(pipeline)
                    .WithGlue(entry.Glue)
                    .WithMetafile(entry.Metafile)
                    .Build();

            entry.Metashare =
                new MetashareBuilder()
                    .WithHash(entry.Hash)
                    .WithPipeline(pipeline)
                    .WithGlue(entry.Glue)
                    .WithMetafile(entry.Metafile)
                    .Build();

            entry.Repository =
                new RepositoryBuilder()
                    .WithHash(entry.Hash)
                    .WithDestination(entry.Destination)
                    .WithFiles(files)
                    .WithPipeline(pipeline)
                    .Build();

            entry.Omnibus =
                new OmnibusBuilder()
                    .WithHash(entry.Hash)
                    .WithPipeline(pipeline)
                    .Build();

            entry.Retriever =
                new RetrieverBuilder()
                    .WithHash(entry.Hash)
                    .WithPipeline(pipeline)
                    .WithGlue(entry.Glue.ToRetriever())
                    .WithRepository(entry.Repository.ToRetriever())
                    .WithOmnibus(entry.Omnibus.ToRetriver())
                    .Build();

            entry.Datashare =
                new DatashareBuilder()
                    .WithHash(entry.Hash)
                    .WithGlue(entry.Glue)
                    .WithRepository(entry.Repository)
                    .Build();

            entry.Spartan =
                new SpartanBuilder()
                    .WithHash(entry.Hash)
                    .WithPipeline(pipeline)
                    .WithFiles(files)
                    .WithGlue(entry.Glue)
                    .WithMetaget(entry.Metaget)
                    .WithMetashare(entry.Metashare)
                    .WithRepository(entry.Repository)
                    .WithRetriever(entry.Retriever)
                    .WithDatashare(entry.Datashare)
                    .WithGoal(Goal.All)
                    .Build();

            entry.Connector =
                new PeerConnectorBuilder()
                    .WithPipeline(pipeline)
                    .WithNetwork(network)
                    .Build();

            AttachHooks(entry);

            entry.Spartan.Start();
            entry.Connector.Start();
            entry.Metafile.Start();
            entry.Repository.Start();

            negotiatorContext.Hashes.Add(entry.Hash);

            foreach (PeerAddress peer in registrant.Peers)
            {
                entry.Connector.ConnectTo(registrant.Hash, peer);
            }
        }

        private void AttachHooks(LeakEntry entry)
        {
            entry.Glue.Hooks.OnPeerConnected += data =>
            {
                hooks.OnPeerConnected?.Invoke(data);
            };

            entry.Glue.Hooks.OnExtensionListReceived += data =>
            {
                entry.PeersPlugin.HandlePeerConnected(entry.Glue, data);
            };

            entry.PeersPlugin.Hooks.OnPeersDataReceived += data =>
            {
                hooks.OnPeerListReceived?.Invoke(data);

                foreach (PeerAddress remote in data.Remotes)
                {
                    entry.Connector.ConnectTo(data.Hash, remote);
                }
            };

            entry.Glue.Hooks.OnPeerChanged += entry.Omnibus.Handle;
            entry.Glue.Hooks.OnBlockReceived += entry.Retriever.Handle;

            entry.Connector.Hooks.OnConnectionEstablished += data => OnConnectionEstablished(data, entry);
            entry.Negotiator.Hooks.OnHandshakeCompleted += OnHandshakeCompleted;

            entry.Metaget.Hooks.OnMetadataDiscovered += entry.Omnibus.Handle;
            entry.Metaget.Hooks.OnMetadataDiscovered += entry.Repository.Handle;
            entry.Metaget.Hooks.OnMetadataDiscovered += entry.Spartan.Handle;
            entry.Metaget.Hooks.OnMetadataDiscovered += hooks.OnMetadataDiscovered;

            entry.Metafile.Hooks.OnMetafileVerified += entry.Glue.Handle;

            entry.MetadataPlugin.Hooks.OnMetadataMeasured += entry.Metaget.Handle;
            entry.MetadataPlugin.Hooks.OnMetadataPieceReceived += entry.Metaget.Handle;
            entry.MetadataPlugin.Hooks.OnMetadataRequestReceived += entry.Metashare.Handle;

            entry.Repository.Hooks.OnDataVerified += entry.Spartan.Handle;
            entry.Repository.Hooks.OnDataVerified += hooks.OnDataVerified;
            entry.Repository.Hooks.OnDataVerified += entry.Omnibus.Handle;
        }

        private NetworkPoolHooks CreateNetworkHooks()
        {
            return new NetworkPoolHooks
            {
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
            collections.Dispose();
        }
    }
}