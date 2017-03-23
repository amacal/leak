using Leak.Common;
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
using Leak.Tasks;
using System.IO;
using System.Threading.Tasks;
using Leak.Client.Notifications;
using Leak.Completion;
using Leak.Memory;
using Leak.Memory.Events;
using Leak.Networking;

namespace Leak.Client.Peer
{
    public class PeerConnect
    {
        public CompletionWorker Worker { get; set; }
        public PipelineService Pipeline { get; set; }
        public FileFactory Files { get; set; }

        public NetworkPool Network { get; set; }
        public MemoryService Memory { get; set; }

        public FileHash Hash { get; set; }
        public PeerHash Peer { get; set; }
        public PeerHash Localhost { get; set; }
        public PeerAddress Address { get; set; }
        public Metainfo Metainfo { get; set; }

        public NotificationCollection Notifications { get; set; }
        public TaskCompletionSource<PeerSession> Completion { get; set; }

        public PeerConnector Connector { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }

        public MetafileService MetaStore { get; set; }
        public MetagetService MetaGet { get; set; }

        public RepositoryService DataStore { get; set; }
        public DataGetService DataGet { get; set; }
        public OmnibusService DataMap { get; set; }

        public void Start()
        {
            StartMemory();
            StartNetwork();
            StartNegotiator();
            StartConnector();
            StartGlue();
            StartDataMap();
        }

        public void Connect(PeerAddress remote)
        {
            Connector.ConnectTo(Hash, remote);
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
                    .WithPlugin(new MetadataPlugin(metadata))
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
                OnBlockWritten = OnBlockWritten,
                OnPieceAccepted = OnPieceAccepted,
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
                OnBlockRequested = OnBlockRequestSent
            };

            DataGet =
                new DataGetBuilder()
                    .WithHash(Hash)
                    .WithStrategy("sequential")
                    .WithGlue(Glue.AsDataGet())
                    .WithPipeline(Pipeline)
                    .WithRepository(DataStore.AsDataGet())
                    .WithOmnibus(DataMap.AsDataGet())
                    .Build(hooks);

            DataGet.Start();
        }

        public void OnConnectionEstablished(ConnectionEstablished data)
        {
            Negotiator.Start(data.Connection, new HandshakeNegotiatorActiveInstance(Localhost, Hash, HandshakeOptions.Extended));
        }

        public void OnConnectionRejected(ConnectionRejected data)
        {
        }

        public void OnHandshakeCompleted(HandshakeCompleted data)
        {
            Glue?.Connect(data.Connection, data.Handshake);
        }

        public void OnHandshakeRejected(HandshakeRejected data)
        {
        }

        private void OnPeerConnected(PeerConnected data)
        {
            Peer = data.Peer;
            Completion.SetResult(new PeerSession(this));

            DataMap?.Handle(data);
        }

        private void OnPeerDisconnected(PeerDisconnected data)
        {
            Notifications.Enqueue(new PeerDisconnectedNotification(data.Peer));
            DataMap?.Handle(data);
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

        private void OnBlockDataReceived(BlockReceived data)
        {
            DataGet?.Handle(data);
        }

        private void OnBlockRequestReceived(BlockRequested data)
        {
        }

        private void OnBlockRequestSent(BlockRequested data)
        {
        }

        private void OnMetadataMeasured(MetadataMeasured data)
        {
            MetaGet?.Handle(data);
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
        }

        private void OnDataCompleted(DataCompleted data)
        {
            Notifications.Enqueue(new DataCompletedNotification(data.Hash));
        }

        private void OnBlockWritten(BlockWritten data)
        {
            DataGet?.Handle(data);
        }

        private void OnPieceAccepted(PieceAccepted data)
        {
            DataGet?.Handle(data);
        }

        private void OnPieceRejected(PieceRejected data)
        {
            DataGet?.Handle(data);
        }

        private void OnPieceCompleted(PieceCompleted data)
        {
            Notifications.Enqueue(new PieceCompletedNotification(data.Hash, new PieceInfo(data.Piece)));
        }

        private void OnMemorySnapshot(MemorySnapshot data)
        {
            Notifications.Enqueue(new MemorySnapshotNotification(data.Allocation));
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            Glue?.Disconnect(data.Connection);
        }

        private void OnDataChanged(DataChanged data)
        {
            Notifications.Enqueue(new DataChangedNotification(data.Hash, data.Completed));
        }
    }
}