using FluentAssertions;
using Leak.Common;
using Leak.Sockets;
using Leak.Testing;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using Leak.Networking.Core;

namespace Leak.Networking.Tests
{
    public class NetworkTests
    {
        [Test]
        public void ShouldCreateNewTcpSocket()
        {
            using (NetworkFixture fixture = new NetworkFixture())
            using (TcpSocket socket = fixture.Pool.New())
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

            using (NetworkFixture fixture = new NetworkFixture())
            using (TcpSocket socket = fixture.Pool.New())
            {
                NetworkConnection connection = fixture.Pool.Create(socket, direction, remote);

                connection.Should().NotBeNull();
                connection.Direction.Should().Be(direction);
                connection.Remote.Should().Be(NetworkAddress.Parse(remote));
                connection.Identifier.Should().BeGreaterThan(0);
            }
        }

        [Test]
        public void ShouldCreateChangedConnection()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (NetworkFixture fixture = new NetworkFixture())
            using (TcpSocket socket = fixture.Pool.New())
            {
                NetworkConnection origin = fixture.Pool.Create(socket, direction, remote);
                NetworkConnection changed = fixture.Pool.Change(origin, new NetworkConfiguration());

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

            using (NetworkFixture fixture = new NetworkFixture())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionAttached, data =>
                {
                    data.Remote.Should().Be(NetworkAddress.Parse(remote));
                    data.Connection.Should().NotBeNull();
                });

                using (TcpSocket socket = fixture.Pool.New())
                {
                    fixture.Pool.Create(socket, direction, remote);
                }

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerConnectionTerminatedWhenTerminated()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (NetworkFixture fixture = new NetworkFixture())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionTerminated, data =>
                {
                    data.Remote.Should().Be(NetworkAddress.Parse(remote));
                    data.Connection.Should().NotBeNull();
                });

                using (TcpSocket socket = fixture.Pool.New())
                {
                    fixture.Pool.Create(socket, direction, remote).Terminate();
                }

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerConnectionTerminatedWhenReceiving()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (NetworkFixture fixture = new NetworkFixture())
            {
                using (TcpSocket host = fixture.Pool.New())
                using (TcpSocket socket = fixture.Pool.New())
                {
                    TcpSocketInfo info = host.BindAndInfo();
                    int port = info.Endpoint.Port;
                    IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                    socket.Bind();
                    host.Listen(10);

                    Task<TcpSocketAccept> task = host.Accept();
                    TcpSocketConnect connect = await socket.Connect(endpoint);
                    TcpSocketAccept accept = await task;

                    connect.Status.Should().Be(SocketStatus.OK);
                    accept.Status.Should().Be(SocketStatus.OK);
                    accept.Connection.Dispose();

                    NetworkConnection connection = fixture.Pool.Create(socket, direction, endpoint);
                    Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionTerminated, data =>
                    {
                        data.Remote.Should().Be(NetworkAddress.Parse(endpoint));
                        data.Connection.Should().NotBeNull();
                    });

                    connection.Receive(new NullReceiver());
                    handler.Wait().Should().BeTrue();
                }
            }
        }

        [Test]
        public async Task ShouldTriggerConnectionTerminatedWhenSending()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;

            using (NetworkFixture fixture = new NetworkFixture())
            using (TcpSocket host = fixture.Pool.New())
            using (TcpSocket socket = fixture.Pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                int port = info.Endpoint.Port;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                socket.Bind();
                host.Listen(10);

                Task<TcpSocketAccept> task = host.Accept();
                TcpSocketConnect connect = await socket.Connect(endpoint);
                TcpSocketAccept accept = await task;

                connect.Status.Should().Be(SocketStatus.OK);
                accept.Status.Should().Be(SocketStatus.OK);
                accept.Connection.Dispose();

                NetworkConnection connection = fixture.Pool.Create(socket, direction, endpoint);
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionTerminated, data =>
                {
                    data.Remote.Should().Be(NetworkAddress.Parse(endpoint));
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

            using (NetworkFixture fixture = new NetworkFixture())
            using (TcpSocket host = fixture.Pool.New())
            using (TcpSocket socket = fixture.Pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                int port = info.Endpoint.Port;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                socket.Bind();
                host.Listen(10);
                host.Accept(null);

                await socket.Connect(endpoint);

                NetworkConnection connection = fixture.Pool.Create(socket, direction, endpoint);
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionSent, data =>
                {
                    data.Remote.Should().Be(NetworkAddress.Parse(endpoint));
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

            using (NetworkFixture fixture = new NetworkFixture())
            using (TcpSocket host = fixture.Pool.New())
            using (TcpSocket socket = fixture.Pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                int port = info.Endpoint.Port;
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                socket.Bind();
                host.Listen(10);

                Task<TcpSocketAccept> task = host.Accept();
                await socket.Connect(endpoint);
                TcpSocketAccept accept = await task;

                NetworkConnection connection = fixture.Pool.Create(socket, direction, endpoint);
                NetworkBlock block = new NetworkBlock(new byte[1024], 0, message.Length);

                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionReceived, data =>
                {
                    data.Remote.Should().Be(NetworkAddress.Parse(endpoint));
                    data.Connection.Should().NotBeNull();
                    data.Bytes.Should().Be(message.Length);
                });

                connection.Receive(new NullReceiver());
                message.ToBytes(block);

                block.With((buffer, offset, count) =>
                {
                    accept.Connection.Send(new SocketBuffer(buffer, offset, count), null);
                });

                handler.Wait().Should().BeTrue();
            }
        }
    }
}