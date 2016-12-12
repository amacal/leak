using FluentAssertions;
using Leak.Common;
using Leak.Sockets;
using Leak.Testing;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Networking.Tests
{
    public class NetworkTests
    {
        private NetworkFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new NetworkFixture();
            fixture.Start();
        }

        [TearDown]
        public void TearDown()
        {
            fixture.Stop();
        }

        public NetworkPool Pool
        {
            get { return fixture.Pool; }
        }

        public NetworkPoolHooks Hooks
        {
            get { return fixture.Hooks; }
        }

        [Test]
        public void ShouldCreateNewTcpSocket()
        {
            using (TcpSocket socket = Pool.New())
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

            using (TcpSocket socket = Pool.New())
            {
                NetworkConnection connection = Pool.Create(socket, direction, remote);

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

            using (TcpSocket socket = Pool.New())
            {
                NetworkConnection origin = Pool.Create(socket, direction, remote);
                NetworkConnection changed = Pool.Change(origin, new NetworkConfiguration());

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

            Trigger handler = Trigger.Bind(ref Hooks.OnConnectionAttached, data =>
            {
                data.Remote.Should().Be(PeerAddress.Parse(remote));
                data.Connection.Should().NotBeNull();
            });

            using (TcpSocket socket = Pool.New())
            {
                Pool.Create(socket, direction, remote);
            }

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerConnectionTerminatedWhenTerminated()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            Trigger handler = Trigger.Bind(ref Hooks.OnConnectionTerminated, data =>
            {
                data.Remote.Should().Be(PeerAddress.Parse(remote));
                data.Connection.Should().NotBeNull();
            });

            using (TcpSocket socket = Pool.New())
            {
                Pool.Create(socket, direction, remote).Terminate();
            }

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public async Task ShouldTriggerConnectionTerminatedWhenReceiving()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (TcpSocket host = Pool.New())
            using (TcpSocket socket = Pool.New())
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

                NetworkConnection connection = Pool.Create(socket, direction, endpoint);
                Trigger handler = Trigger.Bind(ref Hooks.OnConnectionTerminated, data =>
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

            using (TcpSocket host = Pool.New())
            using (TcpSocket socket = Pool.New())
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

                NetworkConnection connection = Pool.Create(socket, direction, endpoint);
                Trigger handler = Trigger.Bind(ref Hooks.OnConnectionTerminated, data =>
                {
                    data.Remote.Should().Be(PeerAddress.Parse(endpoint));
                    data.Connection.Should().NotBeNull();
                });

                for (int i = 0; i < 10; i++)
                {
                    connection.Send(new OneByteMessage());
                }

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerConnectionSentWhenSentSomeBytes()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;
            NetworkOutgoingMessage message = new RandomMessage(113);

            using (TcpSocket host = Pool.New())
            using (TcpSocket socket = Pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                int port = info.Endpoint.Port;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                socket.Bind();
                host.Listen(10);
                host.Accept(null);

                await socket.Connect(endpoint);

                NetworkConnection connection = Pool.Create(socket, direction, endpoint);
                Trigger handler = Trigger.Bind(ref Hooks.OnConnectionSent, data =>
                {
                    data.Remote.Should().Be(PeerAddress.Parse(endpoint));
                    data.Connection.Should().Be(connection);
                    data.Bytes.Should().Be(message.Length);
                });

                connection.Send(message);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerConnectionReceivedWhenReceivedSomeBytes()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;
            NetworkOutgoingMessage message = new RandomMessage(113);

            using (TcpSocket host = Pool.New())
            using (TcpSocket socket = Pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                int port = info.Endpoint.Port;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                socket.Bind();
                host.Listen(10);

                Task<TcpSocketAccept> task = host.Accept();
                await socket.Connect(endpoint);
                TcpSocketAccept accept = await task;

                NetworkConnection connection = Pool.Create(socket, direction, endpoint);

                Trigger handler = Trigger.Bind(ref Hooks.OnConnectionReceived, data =>
                {
                    data.Remote.Should().Be(PeerAddress.Parse(endpoint));
                    data.Connection.Should().NotBeNull();
                    data.Bytes.Should().Be(message.Length);
                });

                connection.Receive(new NullReceiver());
                accept.Connection.Send(new TcpSocketBuffer(message.ToBytes()), null);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}