using System;
using System.Net;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Networking;
using Leak.Networking.Core;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Peer.Sender.Tests
{
    public class SenderFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;

        private readonly SenderHooks hooks;

        public SenderFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool =
                new NetworkPoolBuilder()
                    .WithPipeline(pipeline)
                    .WithWorker(worker)
                    .WithMemory(new SenderMemory())
                    .Build();

            pool.Start();

            hooks = new SenderHooks();
        }

        public async Task<SenderSession> Start()
        {
            int? port;
            IPEndPoint endpoint;

            TcpSocket client = pool.New();
            TcpSocket server = pool.New();

            server.Bind(out port);
            server.Listen(1);
            endpoint = new IPEndPoint(IPAddress.Loopback, port.Value);

            client.Bind();
            client.Connect(endpoint, null);

            PeerHash peer = PeerHash.Random();
            TcpSocketAccept accepted = await server.Accept();
            SenderConfiguration configuration = new SenderConfiguration { Definition = new SenderMessages() };

            NetworkConnection sender = pool.Create(client, NetworkDirection.Outgoing, endpoint);
            NetworkConnection receiver = pool.Create(accepted.Connection, NetworkDirection.Incoming, accepted.GetRemote());

            SenderService communicator = new SenderService(peer, sender, hooks, configuration);
            return new SenderSession(communicator, sender, receiver);
        }

        public SenderHooks Hooks
        {
            get { return hooks; }
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}