using FluentAssertions;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Communicator;
using Leak.Core.Connector;
using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Messages;
using Leak.Core.Network;
using Leak.Core.Tests.Core;
using NUnit.Framework;

namespace Leak.Core.Tests.Components
{
    public class CommunicatorTests
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;
        private NetworkPool pool;
        private PeerConnector connector;
        private PeerListener listener;
        private CommunicatorService communicator;
        private CommunicatorHooks hooks;
        private CommunicatorConfiguration configuration;
        private ConnectionLoop loopy;
        private Trigger<HandshakeCompleted> connected;
        private BufferedBlockFactory blocks;

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
                    communicator = new CommunicatorService(data.Handshake.Remote, data.Connection, hooks, configuration);
                }
            };

            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            pool = new NetworkPool(worker, new NetworkPoolHooks());
            listener = new PeerListener(pool, listenerHooks, new PeerListenerConfiguration());
            connector = new PeerConnector(pool, connectorHooks, new PeerConnectorConfiguration());
            blocks = new BufferedBlockFactory();

            hooks = new CommunicatorHooks();
            configuration = new CommunicatorConfiguration();
            loopy = new ConnectionLoop(new BufferedBlockFactory(), new ConnectionLoopHooks(), new ConnectionLoopConfiguration());

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
        public void ShouldTriggerMessageSentWhenKeepAlive()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("keep-alive");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendKeepAlive();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageSentWhenChoke()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("choke");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendChoke();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageSentWhenUnchoke()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("unchoke");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendUnchoke();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageSentWhenInterested()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("interested");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendInterested();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageSentWhenHave()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("have");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendHave(2);

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageSentWhenBitfield()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("bitfield");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendBitfield(new Bitfield(20));

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageSentWhenPiece()
        {
            FileHash hash = FileHash.Random();
            DataBlock block = blocks.New(10, null);

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("piece");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendPiece(new Piece(1, 2, block));

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageSentWhenExtended()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageSent.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("extended");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageSent = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            connected.Wait();
            communicator.SendExtended(new Extended(17, new byte[2]));

            handler.Wait().Should().BeTrue();
        }
    }
}