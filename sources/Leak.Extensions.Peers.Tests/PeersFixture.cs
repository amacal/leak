using Leak.Common;
using Leak.Completion;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;

        public PeersFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool = new NetworkPoolBuilder().WithPipeline(pipeline).WithWorker(worker).Build();
            pool.Start();
        }

        public async Task<PeersSession> Start()
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

            FileHash hash = FileHash.Random();
            TcpSocketAccept accepted = await server.Accept();

            NetworkConnection sender = pool.Create(client, NetworkDirection.Outgoing, endpoint);
            NetworkConnection receiver = pool.Create(accepted.Connection, NetworkDirection.Incoming, accepted.GetRemote());

            PeerHash pLeft = PeerHash.Random();
            PeerHash pRight = PeerHash.Random();

            Handshake hLeft = new Handshake(pLeft, pRight, hash, HandshakeOptions.Extended);
            PeersSide sLeft = new PeersSide(sender, hLeft);

            Handshake hRight = new Handshake(pRight, pLeft, hash, HandshakeOptions.Extended);
            PeersSide sRight = new PeersSide(receiver, hRight);

            return new PeersSession(hash, sLeft, sRight);
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}