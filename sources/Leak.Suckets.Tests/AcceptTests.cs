using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Suckets.Tests
{
    public class AcceptTests
    {
        [Test]
        public async Task CanObtainAcceptedLocalEndpointUsingTasks()
        {
            using (CompletionImpl worker = new CompletionImpl())
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
            using (CompletionImpl worker = new CompletionImpl())
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
            using (CompletionImpl worker = new CompletionImpl())
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