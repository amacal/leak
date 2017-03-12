using Leak.Common;
using Leak.Connector;
using Leak.Dataget;
using Leak.Datamap;
using Leak.Datastore;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Files;
using Leak.Glue;
using Leak.Metafile;
using Leak.Metaget;
using Leak.Negotiator;
using Leak.Tasks;
using System.IO;
using System.Threading.Tasks;

namespace Leak.Client.Peer
{
    public class PeerConnect
    {
        public PipelineService Pipeline { get; set; }
        public FileFactory Files { get; set; }
        public DataBlockFactory Blocks { get; set; }

        public FileHash Hash { get; set; }
        public PeerHash Peer { get; set; }
        public PeerHash Localhost { get; set; }
        public PeerAddress Address { get; set; }
        public Metainfo Metainfo { get; set; }

        public PeerCollection Notifications { get; set; }
        public TaskCompletionSource<PeerSession> Completion { get; set; }

        public PeerConnector Connector { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }

        public MetafileService MetaStore { get; set; }
        public MetagetService MetaGet { get; set; }

        public RepositoryService DataStore { get; set; }
        public RetrieverService DataGet { get; set; }
        public OmnibusService DataMap { get; set; }

        public void Download(string destination)
        {
            StartMetaStore(destination);
            StartMetaGet();
            StartDataStore(destination);
            StartDataMap();
            StartDataGet();
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
                    .Build(hooks);

            DataStore.Start();
        }

        private void StartDataMap()
        {
            OmnibusHooks hooks = new OmnibusHooks
            {
                OnBlockReserved = OnBlockReserved,
                OnPieceReady = OnPieceReady,
                OnPieceCompleted = OnPieceCompleted,
                OnDataCompleted = OnDataCompleted,
                OnThresholdReached = OnThresholdReached
            };

            DataMap =
                new OmnibusBuilder()
                    .WithHash(Hash)
                    .WithPipeline(Pipeline)
                    .WithSchedulerThreshold(160)
                    .Build(hooks);

            DataMap.Start();
        }

        private void StartDataGet()
        {
            RetrieverHooks hooks = new RetrieverHooks
            {
                OnBlockRequested = OnBlockRequestSent
            };

            DataGet =
                new RetrieverBuilder()
                    .WithHash(Hash)
                    .WithGlue(Glue.AsDataGet())
                    .WithPipeline(Pipeline)
                    .WithRepository(DataStore.AsDataGet())
                    .WithOmnibus(DataMap.AsDataGet())
                    .Build(hooks);

            DataGet.Start();
        }

        public void OnConnectionEstablished(ConnectionEstablished data)
        {
            Negotiator.Start(data.Connection, new HandshakeNegotiatorActiveInstance(Hash, Localhost, HandshakeOptions.Extended));
        }

        public void OnConnectionRejected(ConnectionRejected data)
        {
        }

        public void OnHandshakeCompleted(HandshakeCompleted data)
        {
            MetadataHooks metadata = new MetadataHooks
            {
                OnMetadataMeasured = OnMetadataMeasured,
                OnMetadataPieceReceived = OnMetadataPieceReceived
            };

            Glue =
                new GlueBuilder()
                    .WithHash(Hash)
                    .WithBlocks(Blocks)
                    .WithPlugin(new MetadataPlugin(metadata))
                    .Build(new GlueHooks
                    {
                        OnPeerConnected = OnPeerConnected,
                        OnPeerDisconnected = OnPeerDisconnected,
                        OnPeerBitfieldChanged = OnPeerBitfieldChanged,
                        OnPeerStatusChanged = OnPeerStatusChanged,
                        OnBlockReceived = OnBlockDataReceived,
                        OnBlockRequested = OnBlockRequestReceived
                    });

            Glue.Connect(data.Connection, data.Handshake);
        }

        public void OnHandshakeRejected(HandshakeRejected data)
        {
        }

        private void OnPeerConnected(PeerConnected data)
        {
            Peer = data.Peer;
            Completion.SetResult(new PeerSession(this));
        }

        private void OnPeerDisconnected(PeerDisconnected data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.PeerDisconnected
            };

            Notifications.Enqueue(notification);
        }

        private void OnPeerBitfieldChanged(PeerBitfieldChanged data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.PeerBitfieldChanged,
                Bitfield = data.Bitfield
            };

            Notifications.Enqueue(notification);
            DataMap?.Handle(data);
        }

        private void OnPeerStatusChanged(PeerStatusChanged data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.PeerStatusChanged,
                State = data.State
            };

            Notifications.Enqueue(notification);
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
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.MetafileReceived,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
            MetaGet?.Handle(data);
        }

        private void OnMetafileVerified(MetafileVerified data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.MetafileCompleted,
                Metainfo = data.Metainfo
            };

            Metainfo = data.Metainfo;
            Notifications.Enqueue(notification);

            DataStore?.Handle(data);
            DataMap?.Handle(data);
        }

        private void OnMetafileMeasured(MetafileMeasured data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.MetafileMeasured,
                Size = data.Size
            };

            Notifications.Enqueue(notification);
        }

        private void OnDataAllocated(DataAllocated data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.DataAllocated
            };

            Notifications.Enqueue(notification);
            DataStore?.Verify(new Bitfield(Metainfo.Pieces.Length));
        }

        private void OnDataVerified(DataVerified data)
        {
            DataMap?.Handle(data);
        }

        private void OnDataCompleted(DataCompleted data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.DataCompleted
            };

            Notifications.Enqueue(notification);
        }

        private void OnBlockReserved(BlockReserved data)
        {
            DataGet?.Handle(data);
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

        private void OnPieceReady(PieceReady data)
        {
            DataGet?.Handle(data);
        }

        private void OnPieceCompleted(PieceCompleted data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.PieceCompleted,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
        }

        private void OnThresholdReached(ThresholdReached data)
        {
            DataGet?.Handle(data);
        }
    }
}