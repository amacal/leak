using FluentAssertions;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Core;
using Leak.Core.Listener;
using Leak.Core.Network;
using NUnit.Framework;

namespace Leak.Core.Tests.Network
{
    public class ConnectorTests
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;
        private NetworkPool pool;
        private PeerConnector connector;
        private PeerConnectorHooks hooks;
        private PeerConnectorConfiguration configuration;
        private PeerListener listener;

        [SetUp]
        public void SetUp()
        {
            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            pool = new NetworkPool(worker, new NetworkPoolHooks());
            listener = new PeerListener(pool, new PeerListenerHooks(), new PeerListenerConfiguration());

            hooks = new PeerConnectorHooks();
            configuration = new PeerConnectorConfiguration();
            connector = new PeerConnector(pool, hooks, configuration);

            worker.Start();
            pipeline.Start();
            pool.Start(pipeline);
            connector.Start(pipeline);
            listener.Start();
        }

        [TearDown]
        public void TearDown()
        {
            worker.Dispose();
            pipeline.Stop();
            listener.Stop();
        }

        [Test]
        public void ShouldTriggerConnectionEstablished()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnConnectionEstablished.Trigger(data =>
            {
                data.Remote.Should().NotBeNull();
                data.Remote.Host.Should().Be("127.0.0.1");
                data.Remote.Port.Should().Be(8080);
            });

            hooks.OnConnectionEstablished = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            handler.Verify();
        }

        [Test]
        public void ShouldTriggerConnectionRejected()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnConnectionRejected.Trigger(data =>
            {
                data.Remote.Should().NotBeNull();
                data.Remote.Host.Should().Be("127.0.0.1");
                data.Remote.Port.Should().Be(7999);
            });

            hooks.OnConnectionRejected = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 7999));

            handler.Verify();
        }

        [Test]
        public void ShouldTriggerOnHandshakeCompleted()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnHandshakeCompleted.Trigger(data =>
            {
                data.Connection.Should().NotBeNull();
                data.Handshake.Hash.Should().Be(hash);
            });

            hooks.OnHandshakeCompleted = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            handler.Verify();
        }

        [Test]
        public void ShouldTriggerOnHandshakeRejected()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnHandshakeRejected.Trigger(data =>
            {
                data.Connection.Should().NotBeNull();
            });

            hooks.OnHandshakeRejected = handler;
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            handler.Verify();
        }
    }
}