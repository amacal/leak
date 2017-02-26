using Leak.Completion;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Sockets.Tests
{
    public class AcceptTests
    {
        [Test]
        public async Task CanObtainAcceptedLocalEndpointUsingTasks()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                IPEndPoint endpoint;
                SocketFactory factory = new SocketFactory(worker);

                using (TcpSocket server = factory.Tcp())
                using (TcpSocket client = factory.Tcp())
                {
                    worker.Start();
                    client.Bind();

                    server.Bind(IPAddress.Loopback);
                    server.Listen(1);

                    endpoint = server.Info().Endpoint;

                    Task<TcpSocketAccept> accepting = server.Accept();
                    await client.Connect(endpoint);

                    TcpSocketAccept accepted = await accepting;
                    IPEndPoint local = accepted.GetLocal();

                    Assert.That(local, Is.EqualTo(endpoint));
                }
            }
        }

        [Test]
        public async Task CanObtainAcceptedRemoteEndpointUsingTasks()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                IPEndPoint endpoint;
                SocketFactory factory = new SocketFactory(worker);

                using (TcpSocket server = factory.Tcp())
                using (TcpSocket client = factory.Tcp())
                {
                    worker.Start();
                    client.Bind();

                    server.Bind(IPAddress.Loopback);
                    server.Listen(1);

                    endpoint = server.Info().Endpoint;

                    Task<TcpSocketAccept> accepting = server.Accept();
                    await client.Connect(endpoint);

                    TcpSocketAccept accepted = await accepting;
                    IPEndPoint remote = accepted.GetRemote();

                    Assert.That(remote.Address, Is.EqualTo(endpoint.Address));
                    Assert.That(remote.Port, Is.Not.EqualTo(endpoint.Port));
                    Assert.That(remote.Port, Is.Not.Zero);
                }
            }
        }

        [Test]
        public async Task CanHandleNotBoundSocket()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                SocketFactory factory = new SocketFactory(worker);

                using (TcpSocket socket = factory.Tcp())
                {
                    TcpSocketAccept accepted = await socket.Accept();

                    Assert.That(accepted.Status, Is.Not.EqualTo(SocketStatus.OK));
                }
            }
        }
    }
}