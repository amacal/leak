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
        private readonly PeerRuntime runtime;
        private readonly ConcurrentBag<PeerConnect> online;

        public PeerClient()
        {
            runtime = new PeerFactory(null);
            online = new ConcurrentBag<PeerConnect>();
        }

        public Task<PeerSession> Connect(FileHash hash, PeerAddress address)
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
            foreach (PeerConnect connect in online.ToArray())
            {
                if (connect.Glue?.Disconnect(data.Connection) == true)
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