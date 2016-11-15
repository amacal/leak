using FluentAssertions;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Communicator;
using Leak.Core.Connector;
using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Network;
using NUnit.Framework;

namespace Leak.Core.Tests.Network
{
    public class LoopyTests
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;
        private NetworkPool pool;
        private PeerConnector connector;
        private PeerListener listener;
        private ConnectionLoop loopy;
        private ConnectionLoopHooks hooks;
        private ConnectionLoopConfiguration configuration;
        private CommunicatorService communicator;
        private Trigger<HandshakeCompleted> connected;

        [SetUp]
        public void SetUp()
        {
            PeerConnectorHooks connectorHooks = new PeerConnectorHooks
            {
                OnHandshakeCompleted = connected = new Trigger<HandshakeCompleted>(data =>
                {
                    loopy.StartProcessing(data.Handshake.Remote, data.Connection);
                })
            };

            PeerListenerHooks listenerHooks = new PeerListenerHooks
            {
                OnHandshakeCompleted = data =>
                {
                    communicator = new CommunicatorService(data.Handshake.Remote, data.Connection, new CommunicatorHooks(), new CommunicatorConfiguration());
                }
            };

            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            pool = new NetworkPool(worker, new NetworkPoolHooks());
            listener = new PeerListener(pool, listenerHooks, new PeerListenerConfiguration());
            connector = new PeerConnector(pool, connectorHooks, new PeerConnectorConfiguration());

            hooks = new ConnectionLoopHooks();
            configuration = new ConnectionLoopConfiguration();
            loopy = new ConnectionLoop(new BufferedBlockFactory(), hooks, configuration);

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
        public void ShouldTriggerMessageReceivedWhenKeepAlive()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("keep-alive");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Complete();
            communicator.SendKeepAlive();

            handler.Complete().Should().BeTrue();
        }
    }
}