using Leak.Common;
using Leak.Connector;
using Leak.Events;
using Leak.Negotiator;
using Leak.Networking;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Leak.Client.Peer
{
    public class PeerClient : IDisposable
    {
        private readonly FileHash hash;
        private readonly PeerRuntime runtime;
        private readonly ConcurrentBag<PeerConnect> online;

        public PeerClient(FileHash hash)
        {
            this.hash = hash;

            runtime = new PeerFactory(null);
            online = new ConcurrentBag<PeerConnect>();
        }

        public Task<PeerSession> Connect(PeerAddress address)
        {
            runtime.Start(new NetworkPoolHooks
            {
                OnConnectionTerminated = OnConnectionTerminated
            });

            PeerConnect connect = new PeerConnect
            {
                Hash = hash,
                Address = address,
                Localhost = PeerHash.Random(),
                Notifications = new PeerCollection(),
                Completion = new TaskCompletionSource<PeerSession>(),
                Pipeline = runtime.Pipeline,
                Files = runtime.Files,
                Blocks = runtime.Blocks
            };

            connect.Negotiator =
                new HandshakeNegotiatorBuilder()
                    .WithNetwork(runtime.Network)
                    .Build(new HandshakeNegotiatorHooks
                    {
                        OnHandshakeCompleted = connect.OnHandshakeCompleted,
                        OnHandshakeRejected = connect.OnHandshakeRejected
                    });

            connect.Connector =
                new PeerConnectorBuilder()
                    .WithNetwork(runtime.Network)
                    .WithPipeline(runtime.Pipeline)
                    .Build(new PeerConnectorHooks
                    {
                        OnConnectionEstablished = connect.OnConnectionEstablished,
                        OnConnectionRejected = connect.OnConnectionRejected
                    });

            connect.Start();
            online.Add(connect);

            connect.Connector.Start();
            connect.Connector.ConnectTo(hash, address);

            return connect.Completion.Task;
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            foreach (PeerConnect session in online.ToArray())
            {
                if (session.Glue?.Disconnect(data.Connection) == true)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            runtime.Stop();
        }
    }
}