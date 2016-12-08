using FluentAssertions;
using Leak.Common;
using Leak.Completion;
using Leak.Core.Tests.Core;
using Leak.Events;
using Leak.Networking;
using Leak.Sockets;
using Leak.Tasks;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Core.Tests.Components
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
            pool = new NetworkPool(pipeline, worker, hooks);

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
                NetworkConnection changed = pool.Change(origin, with => { });

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

            var handler = hooks.OnConnectionAttached.Trigger(data =>
            {
                data.Remote.Should().Be(PeerAddress.Parse(remote));
                data.Connection.Should().NotBeNull();
            });

            using (TcpSocket socket = pool.New())
            {
                hooks.OnConnectionAttached = handler;
                pool.Create(socket, direction, remote);
            }

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerConnectionTerminated()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint remote = new IPEndPoint(address, 8080);
            NetworkDirection direction = NetworkDirection.Outgoing;

            var handler = hooks.OnConnectionTerminated.Trigger(data =>
            {
                data.Remote.Should().Be(PeerAddress.Parse(remote));
                data.Connection.Should().NotBeNull();
            });

            using (TcpSocket socket = pool.New())
            {
                hooks.OnConnectionTerminated = handler;
                pool.Create(socket, direction, remote).Terminate();
            }

            handler.Wait().Should().BeTrue();
        }

        [Test, Ignore("")]
        public async Task ShouldTriggerConnectionDropped()
        {
            NetworkDirection direction = NetworkDirection.Outgoing;
            Trigger<ConnectionTerminated> handler;

            using (TcpSocket host = pool.New())
            using (TcpSocket socket = pool.New())
            {
                TcpSocketInfo info = host.BindAndInfo();
                IPEndPoint endpoint = info.Endpoint;

                handler = hooks.OnConnectionTerminated.Trigger(data =>
                {
                    data.Remote.Should().Be(PeerAddress.Parse(endpoint));
                    data.Connection.Should().NotBeNull();
                });

                host.Accept(null);
                hooks.OnConnectionTerminated = handler;

                NetworkConnection connection = pool.Create(socket, direction, endpoint);

                await socket.Connect(endpoint);
                connection.Send(null);
            }

            handler.Wait().Should().BeTrue();
        }
    }
}