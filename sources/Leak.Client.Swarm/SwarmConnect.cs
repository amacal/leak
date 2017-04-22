using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Leak.Client.Notifications;
using Leak.Common;
using Leak.Completion;
using Leak.Connector;
using Leak.Data.Get;
using Leak.Data.Map;
using Leak.Data.Share;
using Leak.Data.Store;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Files;
using Leak.Listener;
using Leak.Listener.Events;
using Leak.Memory;
using Leak.Memory.Events;
using Leak.Meta.Get;
using Leak.Meta.Share;
using Leak.Meta.Store;
using Leak.Networking;
using Leak.Networking.Core;
using Leak.Networking.Events;
using Leak.Peer.Coordinator;
using Leak.Peer.Coordinator.Events;
using Leak.Peer.Negotiator;
using Leak.Peer.Receiver;
using Leak.Peer.Sender;
using Leak.Tasks;
using Leak.Tracker.Get;
using Leak.Tracker.Get.Events;

namespace Leak.Client.Swarm
{
    internal class SwarmConnect
    {
        public SwarmSettings Settings { get; set; }

        public CompletionWorker Worker { get; set; }
        public PipelineService Pipeline { get; set; }
        public FileFactory Files { get; set; }

        public NetworkPool Network { get; set; }
        public MemoryService Memory { get; set; }

        public FileHash Hash { get; set; }
        public PeerHash Localhost { get; set; }
        public Metainfo Metainfo { get; set; }

        public HashSet<PeerHash> Peers { get; set; }
        public HashSet<NetworkAddress> Remotes { get; set; }

        public NotificationCollection Notifications { get; set; }
        public TaskCompletionSource<SwarmSession> Completion { get; set; }

        public TrackerGetService TrackerGet { get; set; }
        public PeerConnector Connector { get; set; }
        public PeerListener Listener { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }

        public ReceiverService Receiver { get; set; }
        public SenderService Sender { get; set; }
        public CoordinatorService Coordinator { get; set; }

        public MetafileService MetaStore { get; set; }
        public MetagetService MetaGet { get; set; }
        public MetashareService MetaShare { get; set; }

        public RepositoryService DataStore { get; set; }
        public OmnibusService DataMap { get; set; }
        public DataGetService DataGet { get; set; }
        public DataShareService DataShare { get; set; }

        public void Start()
        {
            StartMemory();
            StartDataMap();

            StartNetwork();
            StartNegotiator();

            StartSender();
            StartReceiver();
            StartCoordinator();

            StartConnector();
            StartListener();
            StartTrackerGet();

            Completion.SetResult(new SwarmSession(this));
        }

        public void Announce(string[] trackers)
        {
            foreach (string tracker in trackers ?? Enumerable.Empty<string>())
            {
                TrackerGet.Register(new TrackerGetRegistrant
                {
                    Hash = Hash,
                    Address = new Uri(tracker, UriKind.Absolute),
                    Port = Settings.ListenerPort
                });
            }
        }

        public void Download(string destination)
        {
            StartMetaStore(destination);
            StartMetaGet();

            StartDataStore(destination);
            StartDataGet();
        }

        public void Seed(string destination)
        {
            StartMetaStore(destination);
            StartMetaGet();
            StartMetaShare();

            StartDataStore(destination);
            StartDataGet();
            StartDataShare();

            Coordinator.Configuration.AnnounceBitfield = true;
        }

        private void StartMemory()
        {
            MemoryHooks hooks = new MemoryHooks
            {
                OnMemorySnapshot = OnMemorySnapshot
            };

            Memory =
                new MemoryBuilder()
                    .WithMaxRequestSize(256 * 1024)
                    .WithThresholds(20 * 1024)
                    .Build(hooks);
        }

        private void StartNetwork()
        {
            NetworkPoolHooks hooks = new NetworkPoolHooks
            {
                OnConnectionTerminated = OnConnectionTerminated
            };

            Network =
                new NetworkPoolBuilder()
                    .WithPipeline(Pipeline)
                    .WithWorker(Worker)
                    .WithMemory(Memory.AsNetwork())
                    .WithBufferSize(256 * 1024)
                    .Build(hooks);

            Network.Start();
        }

        private void StartNegotiator()
        {
            HandshakeNegotiatorHooks hooks = new HandshakeNegotiatorHooks
            {
                OnHandshakeCompleted = OnHandshakeCompleted
            };

            Negotiator =
                new HandshakeNegotiatorBuilder()
                    .WithNetwork(Network)
                    .Build(hooks);
        }

        private void StartConnector()
        {
            if (Settings.Connector)
            {
                PeerConnectorHooks hooks = new PeerConnectorHooks
                {
                    OnConnectionEstablished = OnConnectionEstablished
                };

                Connector =
                    new PeerConnectorBuilder()
                        .WithNetwork(Network)
                        .WithPipeline(Pipeline)
                        .Build(hooks);

                Connector.Start();
            }
        }

        private void StartSender()
        {
            SenderHooks hooks = new SenderHooks
            {
            };

            Sender =
                new SenderBuilder()
                    .WithHash(Hash)
                    .WithDefinition(new MessageDefinition())
                    .Build(hooks);
        }

        private void StartReceiver()
        {
            ReceiverHooks hooks = new ReceiverHooks
            {
                OnMessageReceived = data => Coordinator?.Handle(data)
            };

            Receiver =
                new ReceiverBuilder()
                    .WithDefinition(new MessageDefinition())
                    .Build(hooks);
        }

        private void StartCoordinator()
        {
            MetadataHooks metadata = new MetadataHooks
            {
                OnMetadataMeasured = data => MetaGet?.Handle(data),
                OnMetadataPieceReceived = OnMetadataPieceReceived,
                OnMetadataRequestSent = OnMetadataRequestSent,
                OnMetadataRequestReceived = OnMetadataRequestReceived
            };

            PeersHooks exchange = new PeersHooks
            {
                OnPeersDataReceived = OnPeerDataReceived
            };

            CoordinatorHooks hooks = new CoordinatorHooks
            {
                OnPeerConnected = OnPeerConnected,
                OnPeerDisconnected = OnPeerDisconnected,
                OnBitfieldChanged = OnPeerBitfieldChanged,
                OnStatusChanged = OnPeerStatusChanged,
                OnBlockReceived = data => DataGet?.Handle(data),
                OnBlockRequested = data => DataShare?.Handle(data),
                OnMessageRequested = data => Sender?.Send(data.Peer, data.Message),
                OnKeepAliveRequested = data => Sender?.SendKeepAlive(data.Peer)
            };

            Coordinator =
                new CoordinatorBuilder()
                    .WithHash(Hash)
                    .WithMemory(Memory)
                    .WithPipeline(Pipeline)
                    .WithMetadata(Settings, metadata)
                    .WithExchange(Settings, exchange)
                    .Build(hooks);

            Coordinator.Start();
        }

        private void StartDataMap()
        {
            OmnibusHooks hooks = new OmnibusHooks
            {
                OnBlockReserved = data => DataGet?.Handle(data),
                OnPieceReady = data => DataGet?.Handle(data),
                OnPieceCompleted = OnPieceCompleted,
                OnThresholdReached = data => DataGet?.Handle(data),
                OnDataCompleted = OnDataCompleted,
                OnDataChanged = OnDataChanged
            };

            DataMap =
                new OmnibusBuilder()
                    .WithHash(Hash)
                    .WithPipeline(Pipeline)
                    .WithSchedulerThreshold(160)
                    .WithPoolSize(512)
                    .Build(hooks);

            DataMap.Start();
        }

        private void StartTrackerGet()
        {
            TrackerGetHooks hooks = new TrackerGetHooks
            {
                OnAnnounced = OnAnnounced
            };

            TrackerGet =
                new TrackerGetBuilder()
                    .WithPipeline(Pipeline)
                    .WithWorker(Worker)
                    .WithPeer(Localhost)
                    .Build(hooks);

            TrackerGet.Start();
        }

        private void StartListener()
        {
            if (Settings.Listener)
            {
                PeerListenerHooks hooks = new PeerListenerHooks
                {
                    OnConnectionArrived = OnConnectionArrived,
                    OnListenerStarted = OnListenerStarted,
                    OnListenerFailed = OnListenerFailed
                };

                Listener =
                    new PeerListenerBuilder()
                        .WithNetwork(Network)
                        .WithPort(Settings)
                        .Build(hooks);

                Listener.Start();
            }
        }

        private void StartMetaStore(string destination)
        {
            MetafileHooks hooks = new MetafileHooks
            {
                OnMetafileVerified = OnMetafileVerified,
                OnMetafileRead = data => MetaShare?.Handle(data)
            };

            MetaStore =
                new MetafileBuilder()
                    .WithHash(Hash)
                    .WithPipeline(Pipeline)
                    .WithFiles(Files)
                    .WithDestination(Path.Combine(destination, $"{Hash}.metainfo"))
                    .Build(hooks);

            MetaStore.Start();
        }

        private void StartMetaGet()
        {
            MetagetHooks hooks = new MetagetHooks
            {
                OnMetafileMeasured = OnMetafileMeasured
            };

            MetaGet =
                new MetagetBuilder()
                    .WithHash(Hash)
                    .WithGlue(Coordinator.AsMetaGet())
                    .WithPipeline(Pipeline)
                    .WithMetafile(MetaStore.AsMetaGet())
                    .Build(hooks);

            MetaGet.Start();
        }

        private void StartMetaShare()
        {
            MetashareHooks hooks = new MetashareHooks
            {
            };

            MetaShare =
                new MetashareBuilder()
                    .WithHash(Hash)
                    .WithGlue(Coordinator)
                    .WithPipeline(Pipeline)
                    .WithMetafile(MetaStore)
                    .Build(hooks);

            MetaShare.Start();
        }

        private void StartDataStore(string destination)
        {
            RepositoryHooks hooks = new RepositoryHooks
            {
                OnDataAllocated = OnDataAllocated,
                OnDataVerified = OnDataVerified,
                OnBlockWritten = data => DataGet?.Handle(data),
                OnBlockRead = data => DataShare?.Handle(data),
                OnPieceAccepted = data => DataGet?.Handle(data),
                OnPieceRejected = OnPieceRejected
            };

            DataStore =
                new RepositoryBuilder()
                    .WithHash(Hash)
                    .WithDestination(Path.Combine(destination, Hash.ToString()))
                    .WithPipeline(Pipeline)
                    .WithFiles(Files)
                    .WithMemory(Memory.AsDataStore())
                    .WithBufferSize(256 * 1024)
                    .Build(hooks);

            DataStore.Start();
        }

        private void StartDataGet()
        {
            DataGetHooks hooks = new DataGetHooks
            {
            };

            DataGet =
                new DataGetBuilder()
                    .WithHash(Hash)
                    .WithStrategy(Settings.Strategy)
                    .WithGlue(Coordinator.AsDataGet())
                    .WithPipeline(Pipeline)
                    .WithDataStore(DataStore.AsDataGet())
                    .WithDataMap(DataMap.AsDataGet())
                    .Build(hooks);

            DataGet.Start();
        }

        private void StartDataShare()
        {
            DataShareHooks hooks = new DataShareHooks
            {
            };

            DataShare =
                new DataShareBuilder()
                    .WithHash(Hash)
                    .WithPipeline(Pipeline)
                    .WithGlue(Coordinator.AsDataShare())
                    .WithDataStore(DataStore.AsDataShare())
                    .WithDataMap(DataMap.AsDataShare())
                    .Build(hooks);

            DataShare.Start();
        }

        private void OnMemorySnapshot(MemorySnapshot data)
        {
            Notifications.Enqueue(new MemorySnapshotNotification(data.Allocation));
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            Coordinator?.Disconnect(data.Connection);
        }

        private void OnHandshakeCompleted(HandshakeCompleted data)
        {
            Coordinator?.Connect(data.Connection, data.Handshake);
        }

        public void OnConnectionEstablished(ConnectionEstablished data)
        {
            Negotiator?.Start(data.Connection, new HandshakeNegotiatorActiveInstance(Localhost, Hash, HandshakeOptions.Extended));
        }

        private void OnConnectionArrived(ConnectionArrived data)
        {
            if (Settings.Filter?.Accept(data.Remote) != false)
            {
                Negotiator.Handle(data.Connection, new HandshakeNegotiatorPassiveInstance(Localhost, Hash, HandshakeOptions.Extended));
            }
            else
            {
                Notifications.Enqueue(new PeerRejectedNotification(data.Remote));
                data.Connection.Terminate();
            }
        }

        private void OnListenerStarted(ListenerStarted data)
        {
            Notifications.Enqueue(new ListenerStartedNotification(Localhost, data.Port));
            Settings.ListenerPort = data.Port;
        }

        private void OnListenerFailed(ListenerFailed data)
        {
            Notifications.Enqueue(new ListenerFailedNotification(Localhost, data.Reason));
        }

        private void OnMetadataPieceReceived(MetadataReceived data)
        {
            Notifications.Enqueue(new MetafileReceivedNotification(data.Hash, new PieceInfo(data.Piece)));
            MetaGet?.Handle(data);
        }

        private void OnMetadataRequestSent(MetadataRequested data)
        {
            Notifications.Enqueue(new MetafileRequestedNotification(data.Hash, new PieceInfo(data.Piece)));
        }

        private void OnMetadataRequestReceived(MetadataRequested data)
        {
            Notifications.Enqueue(new MetafileRequestedNotification(data.Hash, new PieceInfo(data.Piece)));
            MetaShare?.Handle(data);
        }

        private void OnPeerDataReceived(PeersReceived data)
        {
            foreach (NetworkAddress peer in data.Remotes)
            {
                if (Remotes.Add(peer))
                {
                    if (Settings.Filter?.Accept(peer) != false)
                    {
                        Connector?.ConnectTo(Hash, peer);
                    }
                    else
                    {
                        Notifications.Enqueue(new PeerRejectedNotification(peer));
                    }
                }
            }
        }

        private void OnPeerConnected(PeerConnected data)
        {
            Notifications.Enqueue(new PeerConnectedNotification(data.Peer));

            Peers.Add(data.Peer);
            DataMap?.Handle(data);

            Sender?.Add(data.Peer, data.Connection);
            Receiver?.StartProcessing(data.Peer, data.Connection);
        }

        private void OnPeerDisconnected(PeerDisconnected data)
        {
            Notifications.Enqueue(new PeerDisconnectedNotification(data.Peer));

            Peers.Remove(data.Peer);
            Remotes.Remove(data.Remote);
            DataMap?.Handle(data);
            Sender?.Remove(data.Peer);
        }

        private void OnPeerBitfieldChanged(BitfieldChanged data)
        {
            Notifications.Enqueue(new BitfieldChangedNotification(data.Peer, data.Bitfield));
            DataMap?.Handle(data);
        }

        private void OnPeerStatusChanged(StatusChanged data)
        {
            Notifications.Enqueue(new StatusChangedNotification(data.Peer, data.State));
            DataMap?.Handle(data);
        }

        private void OnPieceCompleted(PieceCompleted data)
        {
            Notifications.Enqueue(new PieceCompletedNotification(data.Hash, new PieceInfo(data.Piece)));
        }

        private void OnDataCompleted(DataCompleted data)
        {
            Notifications.Enqueue(new DataCompletedNotification(data.Hash));
        }

        private void OnDataChanged(DataChanged data)
        {
            Notifications.Enqueue(new DataChangedNotification(data.Hash, data.Completed));
        }

        private void OnAnnounced(TrackerAnnounced data)
        {
            foreach (NetworkAddress peer in data.Peers)
            {
                if (Remotes.Add(peer))
                {
                    if (Settings.Filter?.Accept(peer) != false)
                    {
                        Connector?.ConnectTo(Hash, peer);
                    }
                    else
                    {
                        Notifications.Enqueue(new PeerRejectedNotification(peer));
                    }
                }
            }
        }

        private void OnMetafileVerified(MetafileVerified data)
        {
            Metainfo = data.Metainfo;
            Notifications.Enqueue(new MetafileCompletedNotification(Metainfo));

            Coordinator?.Handle(data);
            DataStore?.Handle(data);
            DataMap?.Handle(data);
        }

        private void OnMetafileMeasured(MetafileMeasured data)
        {
            Notifications.Enqueue(new MetafileMeasuredNotification(data.Hash, new Size(data.Size)));
        }

        private void OnDataAllocated(DataAllocated data)
        {
            Notifications.Enqueue(new DataAllocatedNotification(data.Hash, data.Directory));
            DataStore?.Verify(new Bitfield(Metainfo.Pieces.Length));
        }

        private void OnDataVerified(DataVerified data)
        {
            Notifications.Enqueue(new DataVerifiedNotification(data.Hash, data.Bitfield));

            DataMap?.Handle(data);
            DataGet?.Handle(data);
            DataShare?.Handle(data);

            Coordinator?.Handle(data);
            TrackerGet.Announce(Hash);
        }

        private void OnPieceRejected(PieceRejected data)
        {
            Notifications.Enqueue(new PieceRejectedNotification(data.Hash, data.Piece));
            DataGet?.Handle(data);
        }
    }
}