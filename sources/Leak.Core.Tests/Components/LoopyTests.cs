using FluentAssertions;
using Leak.Common;
using Leak.Completion;
using Leak.Core.Communicator;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Messages;
using Leak.Core.Tests.Core;
using Leak.Events;
using Leak.Networking;
using Leak.Tasks;
using NUnit.Framework;

namespace Leak.Core.Tests.Components
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
        private BufferedBlockFactory blocks;
        private int port;

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
                },
                OnListenerStarted = data => port = data.Port
            };

            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            pool = new NetworkPool(pipeline, worker, new NetworkPoolHooks());
            listener = new PeerListener(pool, listenerHooks, new PeerListenerConfiguration
            {
                Port = new PeerListenerPortRandom()
            });

            connector = new PeerConnector(pool, connectorHooks, new PeerConnectorConfiguration());
            blocks = new BufferedBlockFactory();

            hooks = new ConnectionLoopHooks();
            configuration = new ConnectionLoopConfiguration();
            loopy = new ConnectionLoop(new BufferedBlockFactory(), hooks, configuration);

            worker.Start();
            pipeline.Start();
            pool.Start();
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
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendKeepAlive();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageReceivedChoke()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("choke");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendChoke();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageReceivedUnchoke()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("unchoke");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendUnchoke();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageReceivedInterested()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("interested");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendInterested();

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageReceivedHave()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("have");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendHave(2);

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageReceivedBitfield()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("bitfield");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendBitfield(new Bitfield(20));

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageReceivedPiece()
        {
            FileHash hash = FileHash.Random();
            DataBlock block = blocks.New(10, null);

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("piece");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendPiece(new Piece(1, 2, block));

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMessageReceivedExtended()
        {
            FileHash hash = FileHash.Random();

            var handler = hooks.OnMessageReceived.Trigger(data =>
            {
                data.Peer.Should().NotBeNull();
                data.Type.Should().Be("extended");
                data.Payload.Should().NotBeNull();
            });

            hooks.OnMessageReceived = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));

            connected.Wait();
            communicator.SendExtended(new Extended(17, new byte[2]));

            handler.Wait().Should().BeTrue();
        }
    }
}