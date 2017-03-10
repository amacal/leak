using System;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Connector;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Glue;
using Leak.Metadata;
using Leak.Negotiator;

namespace Leak.Client.Peer
{
    public class PeerSession
    {
        public FileHash Hash { get; set; }
        public PeerHash Peer { get; set; }
        public PeerHash Localhost { get; set; }
        public PeerAddress Address { get; set; }

        public PeerCollection Notifications { get; set; }
        public TaskCompletionSource<PeerConnect> Completion { get; set; }

        public PeerConnector Connector { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }

        public byte[] Metadata { get; set; }

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
                        OnPeerChanged = OnPeerChanged,
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
            Completion.SetResult(new PeerConnect(this));
        }

        private void OnPeerDisconnected(PeerDisconnected data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.Disconnected
            };

            Notifications.Enqueue(notification);
        }

        private void OnPeerChanged(PeerChanged data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BitfieldChanged,
                Bitfield = data.Bitfield,
                State = data.State
            };

            Notifications.Enqueue(notification);
        }

        private void OnBlockReceived(BlockReceived data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BlockReceived,
                Index = data.Block
            };

            Notifications.Enqueue(notification);
        }

        private void OnBlockRequested(BlockRequested data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BlockRequested,
                Index = data.Block
            };

            Notifications.Enqueue(notification);
        }

        private void OnMetadataMeasured(MetadataMeasured data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.MetadataMeasured,
                Size = data.Size
            };

            if (Metadata == null)
            {
                Notifications.Enqueue(notification);
                Glue.SendMetadataRequest(Peer, 0);
                Metadata = new byte[data.Size];
            }
        }

        private void OnMetadataPieceReceived(MetadataReceived data)
        {
            Array.Copy(data.Data, 0, Metadata, data.Piece * 16384, data.Data.Length);

            if (data.Piece * 16384 + data.Data.Length == Metadata.Length)
            {
                PeerNotification notification = new PeerNotification
                {
                    Type = PeerNotificationType.MetadataReceived,
                    Metainfo = MetainfoFactory.FromBytes(Metadata)
                };

                Notifications.Enqueue(notification);
            }
            else
            {
                Glue.SendMetadataRequest(Peer, data.Piece + 1);
            }
        }
    }
}