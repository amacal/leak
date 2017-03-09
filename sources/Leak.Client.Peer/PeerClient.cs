using System;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Connector;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Glue;
using Leak.Metadata;
using Leak.Negotiator;
using Leak.Networking;

namespace Leak.Client.Peer
{
    public class PeerClient : IDisposable
    {
        private readonly PeerAddress address;
        private readonly FileHash hash;
        private readonly PeerEntry entry;

        private readonly PeerRuntime runtime;
        private readonly PeerCollection collection;

        private readonly TaskCompletionSource<PeerConnect> completion;

        public PeerClient(PeerAddress address, FileHash hash)
        {
            this.address = address;
            this.hash = hash;

            completion = new TaskCompletionSource<PeerConnect>();
            collection = new PeerCollection();
            runtime = new PeerFactory(null);
            entry = new PeerEntry();
        }

        public Task<PeerConnect> Connect()
        {
            runtime.Start(new NetworkPoolHooks
            {
                OnConnectionTerminated = OnConnectionTerminated
            });

            entry.Connector =
                new PeerConnectorBuilder()
                    .WithNetwork(runtime.Network)
                    .WithPipeline(runtime.Pipeline)
                    .Build(new PeerConnectorHooks
                    {
                        OnConnectionEstablished = OnConnectionEstablished,
                        OnConnectionRejected = OnConnectionRejected
                    });

            entry.Connector.Start();
            entry.Connector.ConnectTo(hash, address);

            return completion.Task;
        }

        public Task<PeerNotification> Next()
        {
            return collection.Next();
        }

        private void OnConnectionEstablished(ConnectionEstablished data)
        {
            entry.Negotiator =
                new HandshakeNegotiatorBuilder()
                    .WithNetwork(runtime.Network)
                    .Build(new HandshakeNegotiatorHooks
                    {
                        OnHandshakeCompleted = OnHandshakeCompleted,
                        OnHandshakeRejected = OnHandshakeRejected
                    });

            entry.Negotiator.Start(data.Connection, new HandshakeNegotiatorActiveInstance(hash, PeerHash.Random(), HandshakeOptions.Extended));
        }

        private void OnConnectionRejected(ConnectionRejected data)
        {
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.Disconnected
            };

            collection.Enqueue(notification);
        }

        private void OnHandshakeCompleted(HandshakeCompleted data)
        {
            MetadataHooks metadata = new MetadataHooks
            {
                OnMetadataMeasured = OnMetadataMeasured,
                OnMetadataPieceReceived = OnMetadataPieceReceived
            };

            entry.Glue =
                new GlueBuilder()
                    .WithBlocks(null)
                    .WithHash(hash)
                    .WithPlugin(new MetadataPlugin(metadata))
                    .Build(new GlueHooks
                    {
                        OnPeerConnected = OnPeerConnected,
                        OnPeerChanged = OnPeerChanged,
                        OnBlockReceived = OnBlockReceived,
                        OnBlockRequested = OnBlockRequested
                    });

            entry.Glue.Connect(data.Connection, data.Handshake);
        }

        private void OnHandshakeRejected(HandshakeRejected data)
        {
        }

        private void OnPeerConnected(PeerConnected data)
        {
            PeerConnect connect = new PeerConnect
            {
                Peer = data.Peer
            };

            entry.Peer = data.Peer;
            completion.SetResult(connect);
        }

        private void OnPeerChanged(PeerChanged data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BitfieldChanged,
                Bitfield = data.Bitfield,
                State = data.State
            };

            collection.Enqueue(notification);
        }

        private void OnBlockReceived(BlockReceived data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BlockReceived,
                Index = data.Block
            };

            collection.Enqueue(notification);
        }

        private void OnBlockRequested(BlockRequested data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.BlockRequested,
                Index = data.Block
            };

            collection.Enqueue(notification);
        }

        private void OnMetadataMeasured(MetadataMeasured data)
        {
            PeerNotification notification = new PeerNotification
            {
                Type = PeerNotificationType.MetadataMeasured,
                Size = data.Size
            };

            if (entry.Metadata == null)
            {
                collection.Enqueue(notification);
                entry.Glue.SendMetadataRequest(entry.Peer, 0);
                entry.Metadata = new byte[data.Size];
            }
        }

        private void OnMetadataPieceReceived(MetadataReceived data)
        {
            Array.Copy(data.Data, 0, entry.Metadata, data.Piece * 16384, data.Data.Length);

            if (data.Piece * 16384 + data.Data.Length == entry.Metadata.Length)
            {
                PeerNotification notification = new PeerNotification
                {
                    Type = PeerNotificationType.MetadataReceived,
                    Metainfo = MetainfoFactory.FromBytes(entry.Metadata)
                };

                collection.Enqueue(notification);
            }
            else
            {
                entry.Glue.SendMetadataRequest(entry.Peer, data.Piece + 1);
            }
        }

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}