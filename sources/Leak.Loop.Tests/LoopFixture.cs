using System;
using System.Net;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Networking;
using Leak.Networking.Core;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Peer.Receiver.Tests
{
    public class LoopFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;

        private readonly ReceiverHooks hooks;
        private readonly LoopSamples samples;

        public LoopFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool =
                new NetworkPoolBuilder()
                    .WithPipeline(pipeline)
                    .WithWorker(worker)
                    .WithMemory(new LoopMemory())
                    .Build();

            pool.Start();

            hooks = new ReceiverHooks();
            samples = new LoopSamples();
        }

        public ReceiverHooks Hooks
        {
            get { return hooks; }
        }

        public LoopSamples Samples
        {
            get { return samples; }
        }

        public async Task<LoopSession> Start()
        {
            int? port;

            ReceiverService loop =
                new ReceiverBuilder()
                    .WithDefinition(new LoopMessages())
                    .Build(hooks);

            TcpSocket client = pool.New();
            TcpSocket server = pool.New();

            client.Bind();
            server.Bind(out port);
            server.Listen(1);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port.Value);
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