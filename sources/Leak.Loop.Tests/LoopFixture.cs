using System;
using System.Net;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Memory;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Loop.Tests
{
    public class LoopFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;

        private readonly ConnectionLoopHooks hooks;
        private readonly LoopSamples samples;

        public LoopFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(new NetworkPoolHooks());
            pool.Start();

            hooks = new ConnectionLoopHooks();
            samples = new LoopSamples();
        }

        public ConnectionLoopHooks Hooks
        {
            get { return hooks; }
        }

        public LoopSamples Samples
        {
            get { return samples; }
        }

        public async Task<LoopSession> Start()
        {
            int port;
            DataBlockFactory factory = new BufferedBlockFactory();

            ConnectionLoopConfiguration configuration = new ConnectionLoopConfiguration();
            ConnectionLoop loop = new ConnectionLoop(factory, hooks, configuration);

            TcpSocket client = pool.New();
            TcpSocket server = pool.New();

            client.Bind();
            server.Bind(out port);
            server.Listen(1);
            
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);
            Task<TcpSocketAccept> accept = server.Accept();

            client.Connect(endpoint, null);

            PeerHash peer = PeerHash.Random();
            TcpSocketAccept accepted = await accept;
            NetworkConnection receiver = pool.Create(accepted.Connection, NetworkDirection.Incoming, accepted.GetRemote());

            loop.StartProcessing(peer, receiver);
            return new LoopSession(client, loop);
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}
