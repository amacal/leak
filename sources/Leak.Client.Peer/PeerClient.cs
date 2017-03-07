using System;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Connector;
using Leak.Events;
using Leak.Glue;
using Leak.Negotiator;
using Leak.Networking;

namespace Leak.Client.Peer
{
    public class PeerClient : IDisposable
    {
        private readonly PeerAddress address;
        private readonly FileHash hash;

        private readonly PeerRuntime runtime;
        private readonly TaskCompletionSource<PeerConnect> completion;
        private readonly PeerCollection collection;

        public PeerClient(PeerAddress address, FileHash hash)
        {
            this.address = address;
            this.hash = hash;

            completion = new TaskCompletionSource<PeerConnect>();
            collection = new PeerCollection();
            runtime = new PeerFactory(null);
        }

        public Task<PeerConnect> Connect()
        {
            runtime.Start(new NetworkPoolHooks
            {
                OnConnectionTerminated = OnConnectionTerminated
            });

            PeerConnector connector =
                new PeerConnectorBuilder()
                    .WithNetwork(runtime.Network)
                    .WithPipeline(runtime.Pipeline)
                    .Build(new PeerConnectorHooks
                    {
                        OnConnectionEstablished = OnConnectionEstablished,
                        OnConnectionRejected = OnConnectionRejected
                    });

            connector.Start();
            connector.ConnectTo(hash, address);

            return completion.Task;
        }

        public Task<PeerNotification> Next()
        {
            return collection.Next();
        }

        private void OnConnectionEstablished(ConnectionEstablished data)
        {
            HandshakeNegotiator negotiator =
                new HandshakeNegotiatorBuilder()
                    .WithNetwork(runtime.Network)
                    .Build(new HandshakeNegotiatorHooks
                    {
                        OnHandshakeCompleted = OnHandshakeCompleted,
                        OnHandshakeRejected = OnHandshakeRejected
                    });

            negotiator.Start(data.Connection, new HandshakeNegotiatorActiveInstance(hash, PeerHash.Random(), HandshakeOptions.Extended));
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
            GlueService glue =
                new GlueBuilder()
                    .WithBlocks(null)
                    .WithHash(hash)
                    .Build(new GlueHooks
                    {
                        OnPeerConnected = OnPeerConnected,
                        OnPeerChanged = OnPeerChanged,
                        OnBlockReceived = OnBlockReceived,
                        OnBlockRequested = OnBlockRequested
                    });

            glue.Connect(data.Connection, data.Handshake);
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

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}