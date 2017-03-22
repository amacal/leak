using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Leak.Client.Notifications;
using Leak.Common;
using Leak.Completion;
using Leak.Connector;
using Leak.Data.Get;
using Leak.Data.Map;
using Leak.Data.Store;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Files;
using Leak.Glue;
using Leak.Listener;
using Leak.Listener.Events;
using Leak.Memory;
using Leak.Memory.Events;
using Leak.Meta.Get;
using Leak.Meta.Store;
using Leak.Negotiator;
using Leak.Networking;
using Leak.Tasks;
using Leak.Tracker.Get;
using Leak.Tracker.Get.Events;

namespace Leak.Client.Swarm
{
    internal class SwarmConnect
    {
        public SwarmSettings Settings { get; set; }

        public CompletionWorker Worker { get; set; }
        public NetworkPool Network { get; set; }
        public PipelineService Pipeline { get; set; }
        public FileFactory Files { get; set; }
        public MemoryService Memory { get; set; }

        public FileHash Hash { get; set; }
        public PeerHash Localhost { get; set; }
        public Metainfo Metainfo { get; set; }

        public HashSet<PeerHash> Peers { get; set; }
        public HashSet<PeerAddress> Remotes { get; set; }

        public NotificationCollection Notifications { get; set; }
        public TaskCompletionSource<SwarmSession> Completion { get; set; }

        public TrackerGetService TrackerGet { get; set; }
        public PeerConnector Connector { get; set; }
        public PeerListener Listener { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }

        public MetafileService MetaStore { get; set; }
        public MetagetService MetaGet { get; set; }

        public RepositoryService DataStore { get; set; }
        public DataGetService DataGet { get; set; }
        public OmnibusService DataMap { get; set; }

        public void Start(string[] trackers)
        {
            StartMemory();
            StartNetwork();
            StartNegotiator();
            StartConnector();
            StartListener();
            StartGlue();
            StartDataMap();
            StartTrackerGet();

            Announce(trackers);
            Completion.SetResult(new SwarmSession(this));
        }

        private void Announce(string[] trackers)
        {
            foreach (string tracker in trackers)
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

        private void StartGlue()
        {
            MetadataHooks metadata = new MetadataHooks
            {
                OnMetadataMeasured = data => MetaGet?.Handle(data),
                OnMetadataPieceReceived = OnMetadataPieceReceived,
                OnMetadataRequestSent = OnMetadataRequestSent
            };

            PeersHooks exchange = new PeersHooks
            {
                OnPeersDataReceived = OnPeerDataReceived
            };

            GlueHooks hooks = new GlueHooks
            {
                OnPeerConnected = OnPeerConnected,
                OnPeerDisconnected = OnPeerDisconnected,
                OnPeerBitfieldChanged = OnPeerBitfieldChanged,
                OnPeerStatusChanged = OnPeerStatusChanged,
                OnBlockReceived = data => DataGet?.Handle(data)
            };

            Glue =
                new GlueBuilder()
                    .WithHash(Hash)
                    .WithMemory(Memory)
                    .WithPipeline(Pipeline)
                    .WithMetadata(Settings, metadata)
                    .WithExchange(Settings, exchange)
                    .Build(hooks);

            Glue.Start();
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
                        .WithPeer(Localhost)
                        .WithNetwork(Network)
                        .WithPort(Settings)
                        .WithExtensions()
                        .Build(hooks);

                Listener.Start();
            }
        }

        private void StartMetaStore(string destination)
        {
            MetafileHooks hooks = new MetafileHooks
            {
                OnMetafileVerified = OnMetafileVerified
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
                    .WithGlue(Glue.AsMetaGet())
                    .WithPipeline(Pipeline)
                    .WithMetafile(MetaStore.AsMetaGet())
                    .Build(hooks);

            MetaGet.Start();
        }

        private void StartDataStore(string destination)
        {
            RepositoryHooks hooks = new RepositoryHooks
            {
                OnDataAllocated = OnDataAllocated,
                OnDataVerified = OnDataVerified,
                OnBlockWritten = data => DataGet?.Handle(data),
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
                    .WithGlue(Glue.AsDataGet())
                    .WithPipeline(Pipeline)
                    .WithRepository(DataStore.AsDataGet())
                    .WithOmnibus(DataMap.AsDataGet())
                    .Build(hooks);

            DataGet.Start();
        }

        private void OnMemorySnapshot(MemorySnapshot data)
        {
            Notifications.Enqueue(new MemorySnapshotNotification(data.Allocation));
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            Glue?.Disconnect(data.Connection);
        }

        private void OnHandshakeCompleted(HandshakeCompleted data)
        {
            if (Glue.Connect(data.Connection, data.Handshake) == false)
            {
                data.Connection.Terminate();
            }
        }

        public void OnConnectionEstablished(ConnectionEstablished data)
        {
            Negotiator.Start(data.Connection, new HandshakeNegotiatorActiveInstance(Localhost, Hash, HandshakeOptions.Extended));
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
            Notifications.Enqueue(new ListenerStartedNotification(data.Peer, data.Port));
            Settings.ListenerPort = data.Port;
        }

        private void OnListenerFailed(ListenerFailed data)
        {
            Notifications.Enqueue(new ListenerFailedNotification(data.Peer, data.Reason));
        }

        private void OnMetadataPieceReceived(MetadataReceived data)
        {
            Notifications.Enqueue(new MetafileReceivedNotification(data.Hash, new PieceInfo(data.Piece)));
            MetaGet?.Handle(data);
        }

        private void OnMetadataRequestSent(MetadataRequested data)
        {
            Notifications.Enqueue(new MetafileReceivedNotification(data.Hash, new PieceInfo(data.Piece)));
        }

        private void OnPeerDataReceived(PeersReceived data)
        {
            foreach (PeerAddress peer in data.Remotes)
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
        }

        private void OnPeerDisconnected(PeerDisconnected data)
        {
            Notifications.Enqueue(new PeerDisconnectedNotification(data.Peer));

            Peers.Remove(data.Peer);
            Remotes.Remove(data.Remote);
        }

        private void OnPeerBitfieldChanged(PeerBitfieldChanged data)
        {
            Notifications.Enqueue(new BitfieldChangedNotification(data.Peer, data.Bitfield));
            DataMap?.Handle(data);
        }

        private void OnPeerStatusChanged(PeerStatusChanged data)
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
            foreach (PeerAddress peer in data.Peers)
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

            Glue?.Handle(data);
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
            TrackerGet.Announce(Hash);
        }

        private void OnPieceRejected(PieceRejected data)
        {
            Notifications.Enqueue(new PieceRejectedNotification(data.Hash, data.Piece));
            DataGet?.Handle(data);
        }
    }
}