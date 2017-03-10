using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Connector;
using Leak.Events;
using Leak.Negotiator;
using Leak.Networking;

namespace Leak.Client.Peer
{
    public class PeerClient : IDisposable
    {
        private readonly FileHash hash;
        private readonly PeerRuntime runtime;
        private readonly ConcurrentBag<PeerSession> all;

        public PeerClient(FileHash hash)
        {
            this.hash = hash;

            runtime = new PeerFactory(null);
            all = new ConcurrentBag<PeerSession>();
        }

        public Task<PeerConnect> Connect(PeerAddress address)
        {
            runtime.Start(new NetworkPoolHooks
            {
                OnConnectionTerminated = OnConnectionTerminated
            });

            PeerSession session = new PeerSession
            {
                Hash = hash,
                Address = address,
                Localhost = PeerHash.Random(),
                Notifications = new PeerCollection(),
                Completion = new TaskCompletionSource<PeerConnect>()
            };

            session.Negotiator =
                new HandshakeNegotiatorBuilder()
                    .WithNetwork(runtime.Network)
                    .Build(new HandshakeNegotiatorHooks
                    {
                        OnHandshakeCompleted = session.OnHandshakeCompleted,
                        OnHandshakeRejected = session.OnHandshakeRejected
                    });

            session.Connector =
                new PeerConnectorBuilder()
                    .WithNetwork(runtime.Network)
                    .WithPipeline(runtime.Pipeline)
                    .Build(new PeerConnectorHooks
                    {
                        OnConnectionEstablished = session.OnConnectionEstablished,
                        OnConnectionRejected = session.OnConnectionRejected
                    });

            session.Connector.Start();
            session.Connector.ConnectTo(hash, address);

            all.Add(session);
            return session.Completion.Task;
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            foreach (PeerSession session in all.ToArray())
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