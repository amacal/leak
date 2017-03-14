using System;
using System.IO;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Connector;
using Leak.Data.Get;
using Leak.Data.Map;
using Leak.Data.Store;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Files;
using Leak.Glue;
using Leak.Meta.Get;
using Leak.Meta.Store;
using Leak.Negotiator;
using Leak.Networking;
using Leak.Tasks;
using Leak.Tracker.Get;
using Leak.Tracker.Get.Events;

namespace Leak.Client.Swarm
{
    public class SwarmConnect
    {
        public CompletionWorker Worker { get; set; }
        public NetworkPool Network { get; set; }
        public PipelineService Pipeline { get; set; }
        public FileFactory Files { get; set; }
        public DataBlockFactory Blocks { get; set; }

        public FileHash Hash { get; set; }
        public PeerHash Localhost { get; set; }
        public Metainfo Metainfo { get; set; }

        public SwarmCollection Notifications { get; set; }
        public TaskCompletionSource<SwarmSession> Completion { get; set; }

        public TrackerGetService TrackerGet { get; set; }
        public PeerConnector Connector { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }

        public MetafileService MetaStore { get; set; }
        public MetagetService MetaGet { get; set; }

        public RepositoryService DataStore { get; set; }
        public RetrieverService DataGet { get; set; }
        public OmnibusService DataMap { get; set; }

        public void Start(string[] trackers)
        {
            StartNegotiator();
            StartConnector();
            StartGlue();
            StartDataMap();
            StartTrackerGet(trackers);

            Completion.SetResult(new SwarmSession(this));
        }

        public void Download(string destination)
        {
            StartMetaStore(destination);
            StartMetaGet();
            StartDataStore(destination);
            StartDataGet();
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

        private void StartGlue()
        {
            MetadataHooks metadata = new MetadataHooks
            {
                OnMetadataMeasured = data => MetaGet?.Handle(data),
                OnMetadataPieceReceived = OnMetadataPieceReceived,
                OnMetadataRequestSent = OnMetadataRequestSent
            };

            Glue =
                new GlueBuilder()
                    .WithHash(Hash)
                    .WithBlocks(Blocks)
                    .WithPipeline(Pipeline)
                    .WithPlugin(new MetadataPlugin(metadata))
                    .Build(new GlueHooks
                    {
                        OnPeerConnected = OnPeerConnected,
                        OnPeerDisconnected = OnPeerDisconnected,
                        OnPeerBitfieldChanged = OnPeerBitfieldChanged,
                        OnPeerStatusChanged = OnPeerStatusChanged,
                        OnBlockReceived = data => DataGet?.Handle(data)
                    });

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
            };

            DataMap =
                new OmnibusBuilder()
                    .WithHash(Hash)
                    .WithPipeline(Pipeline)
                    .WithSchedulerThreshold(160)
                    .Build(hooks);

            DataMap.Start();
        }

        private void StartTrackerGet(string[] trackers)
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

            foreach (string tracker in trackers)
            {
                TrackerGet.Register(new TrackerGetRegistrant
                {
                    Hash = Hash,
                    Address = new Uri(tracker, UriKind.Absolute)
                });
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
                OnPieceRejected = data => DataGet?.Handle(data)
            };

            DataStore =
                new RepositoryBuilder()
                    .WithHash(Hash)
                    .WithDestination(Path.Combine(destination, Hash.ToString()))
                    .WithPipeline(Pipeline)
                    .WithFiles(Files)
                    .Build(hooks);

            DataStore.Start();
        }

        private void StartDataGet()
        {
            RetrieverHooks hooks = new RetrieverHooks
            {
            };

            DataGet =
                new RetrieverBuilder()
                    .WithHash(Hash)
                    .WithStrategy("rarest-first")
                    .WithGlue(Glue.AsDataGet())
                    .WithPipeline(Pipeline)
                    .WithRepository(DataStore.AsDataGet())
                    .WithOmnibus(DataMap.AsDataGet())
                    .Build(hooks);

            DataGet.Start();
        }

        private void OnHandshakeCompleted(HandshakeCompleted data)
        {
            Glue.Connect(data.Connection, data.Handshake);
        }

        public void OnConnectionEstablished(ConnectionEstablished data)
        {
            Negotiator.Start(data.Connection, new HandshakeNegotiatorActiveInstance(Hash, Localhost, HandshakeOptions.Extended));
        }

        private void OnMetadataPieceReceived(MetadataReceived data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.MetafileReceived,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
            MetaGet?.Handle(data);
        }

        private void OnMetadataRequestSent(MetadataRequested data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.MetafileRequested,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
        }

        private void OnPeerConnected(PeerConnected data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.PeerConnected,
                Peer = data.Peer
            };

            Notifications.Enqueue(notification);
            DataMap?.Handle(data);
        }

        private void OnPeerDisconnected(PeerDisconnected data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.PeerDisconnected,
                Peer = data.Peer
            };

            Notifications.Enqueue(notification);
        }

        private void OnPeerBitfieldChanged(PeerBitfieldChanged data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.PeerBitfieldChanged,
                Bitfield = data.Bitfield
            };

            Notifications.Enqueue(notification);
            DataMap?.Handle(data);
        }

        private void OnPeerStatusChanged(PeerStatusChanged data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.PeerStatusChanged,
                State = data.State
            };

            Notifications.Enqueue(notification);
            DataMap?.Handle(data);
        }

        private void OnPieceCompleted(PieceCompleted data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.PieceCompleted,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
        }

        private void OnDataCompleted(DataCompleted data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.DataCompleted
            };

            Notifications.Enqueue(notification);
        }

        private void OnAnnounced(TrackerAnnounced data)
        {
            foreach (PeerAddress peer in data.Peers)
            {
                Connector.ConnectTo(Hash, peer);
            }
        }

        private void OnMetafileVerified(MetafileVerified data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.MetafileCompleted,
                Metainfo = data.Metainfo
            };

            Metainfo = data.Metainfo;
            Notifications.Enqueue(notification);

            Glue?.Handle(data);
            DataStore?.Handle(data);
            DataMap?.Handle(data);
        }

        private void OnMetafileMeasured(MetafileMeasured data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.MetafileMeasured,
                Size = data.Size
            };

            Notifications.Enqueue(notification);
        }

        private void OnDataAllocated(DataAllocated data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.DataAllocated
            };

            Notifications.Enqueue(notification);
            DataStore?.Verify(new Bitfield(Metainfo.Pieces.Length));
        }

        private void OnDataVerified(DataVerified data)
        {
            SwarmNotification notification = new SwarmNotification
            {
                Type = SwarmNotificationType.DataVerified,
                Bitfield = data.Bitfield
            };

            Notifications.Enqueue(notification);
            DataMap?.Handle(data);
        }
    }
}