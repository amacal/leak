using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Leak.Meta.Info;

namespace Leak.Glue.Tests
{
    public class GlueFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;

        public GlueFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool =
                new NetworkPoolBuilder()
                    .WithPipeline(pipeline)
                    .WithWorker(worker)
                    .WithMemory(new GlueMemory())
                    .Build();

            pool.Start();
        }

        public async Task<GlueSession> Start()
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
            GlueSide sLeft = new GlueSide(sender, hLeft);

            Handshake hRight = new Handshake(pRight, pLeft, hash, HandshakeOptions.Extended);
            GlueSide sRight = new GlueSide(receiver, hRight);

            Metainfo metainfo;
            byte[] data = Bytes.Random(20000);

            using (FileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, data);
                builder.AddFile(path);

                metainfo = builder.ToMetainfo();
            }

            return new GlueSession(metainfo, sLeft, sRight);
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}