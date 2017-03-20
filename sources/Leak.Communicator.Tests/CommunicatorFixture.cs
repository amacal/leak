using Leak.Common;
using Leak.Completion;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Communicator.Tests
{
    public class CommunicatorFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;
        private CommunicatorService communicator;

        private readonly CommunicatorHooks hooks;

        public CommunicatorFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool = new NetworkPoolBuilder().WithPipeline(pipeline).WithWorker(worker).Build();
            pool.Start();

            hooks = new CommunicatorHooks();
        }

        public async Task<CommunicatorSession> Start()
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
            CommunicatorConfiguration configuration = new CommunicatorConfiguration();

            NetworkConnection sender = pool.Create(client, NetworkDirection.Outgoing, endpoint);
            NetworkConnection receiver = pool.Create(accepted.Connection, NetworkDirection.Incoming, accepted.GetRemote());

            communicator = new CommunicatorService(peer, sender, hooks, configuration);
            return new CommunicatorSession(communicator, sender, receiver);
        }

        public CommunicatorHooks Hooks
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