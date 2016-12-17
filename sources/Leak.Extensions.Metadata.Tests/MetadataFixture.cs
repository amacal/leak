using System;
using System.Net;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;

        public MetadataFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(new NetworkPoolHooks());
            pool.Start();
        }

        public async Task<MetadataSession> Start()
        {
            int port;
            IPEndPoint endpoint;

            TcpSocket client = pool.New();
            TcpSocket server = pool.New();

            server.Bind(out port);
            server.Listen(1);
            endpoint = new IPEndPoint(IPAddress.Loopback, port);

            client.Bind();
            client.Connect(endpoint, null);

            FileHash hash = FileHash.Random();
            TcpSocketAccept accepted = await server.Accept();

            NetworkConnection sender = pool.Create(client, NetworkDirection.Outgoing, endpoint);
            NetworkConnection receiver = pool.Create(accepted.Connection, NetworkDirection.Incoming, accepted.GetRemote());

            PeerHash pLeft = PeerHash.Random();
            PeerHash pRight = PeerHash.Random();

            Handshake hLeft = new Handshake(pLeft, pRight, hash, HandshakeOptions.Extended);
            MetadataSide sLeft = new MetadataSide(sender, hLeft);

            Handshake hRight = new Handshake(pRight, pLeft, hash, HandshakeOptions.Extended);
            MetadataSide sRight = new MetadataSide(receiver, hRight);

            return new MetadataSession(hash, sLeft, sRight);
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}