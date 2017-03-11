using Leak.Common;
using Leak.Connector;
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

        public FileHash Hash { get; set; }
        public PeerHash Peer { get; set; }
        public PeerHash Localhost { get; set; }
        public PeerAddress Address { get; set; }

        public PeerCollection Notifications { get; set; }
        public TaskCompletionSource<PeerSession> Completion { get; set; }

        public PeerConnector Connector { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }

        public MetafileService MetaStore { get; set; }
        public MetagetService MetaGet { get; set; }

        public void Download(string destination)
        {
            StartMetaStore(destination);
            StartMetaGet();
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
                    .WithBlocks(null)
                    .WithHash(Hash)
                    .WithPlugin(new MetadataPlugin(metadata))
                    .Build(new GlueHooks
                    {
                        OnPeerConnected = OnPeerConnected,
                        OnPeerDisconnected = OnPeerDisconnected,
                        OnPeerBitfieldChanged = OnPeerBitfieldChanged,
                        OnPeerStatusChanged = OnPeerStatusChanged,
                        OnBlockReceived = OnBlockReceived,
                        OnBlockRequested = OnBlockRequested
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
                Type = PeerNotificationType.Disconnected
            };

            Notifications.Enqueue(notification);
        }

        private void OnPeerBitfieldChanged(PeerBitfieldChanged data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BitfieldChanged,
                Bitfield = data.Bitfield
            };

            Notifications.Enqueue(notification);
        }

        private void OnPeerStatusChanged(PeerStatusChanged data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.StatusChanged,
                State = data.State
            };

            Notifications.Enqueue(notification);
        }

        private void OnBlockReceived(BlockReceived data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BlockReceived,
                Block = data.Block
            };

            Notifications.Enqueue(notification);
        }

        private void OnBlockRequested(BlockRequested data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BlockRequested,
                Block = data.Block
            };

            Notifications.Enqueue(notification);
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

            Notifications.Enqueue(notification);
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
    }
}