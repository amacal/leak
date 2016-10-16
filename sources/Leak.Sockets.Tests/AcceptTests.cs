using System.Net;
using System.Threading.Tasks;
using Leak.Suckets;
using NUnit.Framework;

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
                TcpSocketFactory factory = new TcpSocketFactory(worker);

                using (TcpSocket server = factory.Create())
                using (TcpSocket client = factory.Create())
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
                TcpSocketFactory factory = new TcpSocketFactory(worker);

                using (TcpSocket server = factory.Create())
                using (TcpSocket client = factory.Create())
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
                TcpSocketFactory factory = new TcpSocketFactory(worker);

                using (TcpSocket socket = factory.Create())
                {
                    TcpSocketAccept accepted = await socket.Accept();

                    Assert.That(accepted.Status, Is.Not.EqualTo(TcpSocketStatus.OK));
                }
            }
        }
    }
}