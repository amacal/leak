using System;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Negotiator.Tests
{
    public class NegotiatorFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;
        private readonly HandshakeNegotiator negotiator;
        private readonly HandshakeNegotiatorHooks hooks;

        public NegotiatorFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(new NetworkPoolHooks());
            pool.Start();

            hooks = new HandshakeNegotiatorHooks();
            negotiator = new HandshakeNegotiatorFactory(pool).Create(hooks);
        }

        public HandshakeNegotiator Negotiator
        {
            get { return negotiator; }
        }

        public HandshakeNegotiatorHooks Hooks
        {
            get { return hooks; }
        }

        public async Task<NegotiatorFixturePair> Create()
        {
            int port;

            TcpSocket host = pool.New();
            TcpSocket client = pool.New();

            client.Bind();
            host.Bind(out port);
            host.Listen(1);

            Task<TcpSocketAccept> accept = host.Accept();
            Task<TcpSocketConnect> connect = client.Connect(port);

            TcpSocketAccept accepted = await accept;
            TcpSocketConnect connected = await connect;

            NetworkConnection local = pool.Create(connected.Socket, NetworkDirection.Outgoing, connected.Endpoint);
            NetworkConnection remote = pool.Create(accepted.Connection, NetworkDirection.Incoming, accepted.GetRemote());

            return new NegotiatorFixturePair(local, remote);
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}
