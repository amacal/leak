using FluentAssertions;
using Leak.Common;
using Leak.Completion;
using Leak.Sockets;
using Leak.Tasks;
using Leak.Testing;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Networking.Tests
{
    public class NetworkTests
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;
        private NetworkPool pool;
        private NetworkPoolHooks hooks;

        [SetUp]
        public void SetUp()
        {
            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            hooks = new NetworkPoolHooks();
            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(hooks);

            worker.Start();
            pipeline.Start();
            pool.Start();
        }

        [TearDown]
        public void TearDown()
        {
            worker.Dispose();
            pipeline.Stop();
        }

        [Test]
        public void ShouldCreateNewTcpSocket()
        {
            using (TcpSocket socket = pool.New())
            {
                socket.Should().NotBeNull();
            }
        }

        [Test]
        public void ShouldCreateNewConnection()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (TcpSocket socket = pool.New())
            {
                NetworkConnection connection = pool.Create(socket, direction, remote);

                connection.Should().NotBeNull();
                connection.Direction.Should().Be(direction);
                connection.Remote.Should().Be(PeerAddress.Parse(remote));
                connection.Identifier.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public void ShouldCreateChangedConnection()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (TcpSocket socket = pool.New())
            {
                NetworkConnection origin = pool.Create(socket, direction, remote);
                NetworkConnection changed = pool.Change(origin, new NetworkConfiguration());

                changed.Should().NotBeNull();
                changed.Should().NotBeSameAs(origin);
                changed.Identifier.Should().Be(origin.Identifier);
            }
        }

        [Test]
        public void ShouldTriggerConnectionAttached()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            Trigger handler = Trigger.Bind(ref hooks.OnConnectionAttached, data =>
            {
                data.Remote.Should().Be(PeerAddress.Parse(remote));
                data.Connection.Should().NotBeNull();
            });

            using (TcpSocket socket = pool.New())
            {
                pool.Create(socket, direction, remote);
            }

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerConnectionTerminatedWhenTerminated()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            Trigger handler = Trigger.Bind(ref hooks.OnConnectionTerminated, data =>
            {
                data.Remote.Should().Be(PeerAddress.Parse(remote));
                data.Connection.Should().NotBeNull();
            });

            using (TcpSocket socket = pool.New())
            {
                pool.Create(socket, direction, remote).Terminate();
            }

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public async Task ShouldTriggerConnectionTerminatedWhenReceiving()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (TcpSocket host = pool.New())
            using (TcpSocket socket = pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                int port = info.Endpoint.Port;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                socket.Bind();
                host.Listen(10);

                Task<TcpSocketAccept> task = host.Accept();
                TcpSocketConnect connect = await socket.Connect(endpoint);
                TcpSocketAccept accept = await task;

                connect.Status.Should().Be(TcpSocketStatus.OK);
                accept.Status.Should().Be(TcpSocketStatus.OK);
                accept.Connection.Dispose();

                NetworkConnection connection = pool.Create(socket, direction, endpoint);
                Trigger handler = Trigger.Bind(ref hooks.OnConnectionTerminated, data =>
                {
                    data.Remote.Should().Be(PeerAddress.Parse(endpoint));
                    data.Connection.Should().NotBeNull();
                });

                connection.Receive(new NullReceiver());
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerConnectionTerminatedWhenSending()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (TcpSocket host = pool.New())
            using (TcpSocket socket = pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                int port = info.Endpoint.Port;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                socket.Bind();
                host.Listen(10);

                Task<TcpSocketAccept> task = host.Accept();
                TcpSocketConnect connect = await socket.Connect(endpoint);
                TcpSocketAccept accept = await task;

                connect.Status.Should().Be(TcpSocketStatus.OK);
                accept.Status.Should().Be(TcpSocketStatus.OK);
                accept.Connection.Dispose();

                NetworkConnection connection = pool.Create(socket, direction, endpoint);
                Trigger handler = Trigger.Bind(ref hooks.OnConnectionTerminated, data =>
                {
                    data.Remote.Should().Be(PeerAddress.Parse(endpoint));
                    data.Connection.Should().NotBeNull();
                });

                for (int i = 0; i < 10; i++)
                    connection.Send(new RandomMessage());
                handler.Wait().Should().BeTrue();
            }
        }

        private class RandomMessage : NetworkOutgoingMessage
        {
            public int Length
            {
                get { return 100000; }
            }

            public byte[] ToBytes()
            {
                return Bytes.Random(100000);
            }
        }

        private class NullReceiver : NetworkIncomingMessageHandler
        {
            public void OnMessage(NetworkIncomingMessage message)
            {
            }

            public void OnDisconnected()
            {
            }
        }
    }
}