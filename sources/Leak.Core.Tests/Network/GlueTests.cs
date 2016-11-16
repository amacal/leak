using FluentAssertions;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Listener;
using Leak.Core.Network;
using NUnit.Framework;

namespace Leak.Core.Tests.Network
{
    public class GlueTests
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;

        private PeerListener listener;
        private PeerConnector connector;

        private GlueService left;
        private PeerHash leftHash;
        private GlueHooks leftHooks;
        private GlueService right;
        private GlueHooks rightHooks;
        private PeerHash rightHash;

        private Trigger<HandshakeCompleted> leftConnected;
        private Trigger<HandshakeCompleted> rightConnected;

        [SetUp]
        public void SetUp()
        {
            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            NetworkPool pool = new NetworkPool(worker, new NetworkPoolHooks());
            GlueFactory factory = new GlueFactory(new BufferedBlockFactory(), new GlueConfiguration());

            PeerListenerHooks listenerHooks = new PeerListenerHooks
            {
                OnHandshakeCompleted = rightConnected = new Trigger<HandshakeCompleted>(data =>
                {
                    leftHash = data.Handshake.Remote;
                    right = factory.Create(data.Handshake.Hash, rightHooks);
                    right.Connect(data.Connection, data.Handshake);
                })
            };

            PeerConnectorHooks connectorHooks = new PeerConnectorHooks
            {
                OnHandshakeCompleted = leftConnected = new Trigger<HandshakeCompleted>(data =>
                {
                    rightHash = data.Handshake.Remote;
                    left = factory.Create(data.Handshake.Hash, leftHooks);
                    left.Connect(data.Connection, data.Handshake);
                })
            };

            leftHooks = new GlueHooks();
            rightHooks = new GlueHooks();

            connector = new PeerConnector(pool, connectorHooks, new PeerConnectorConfiguration());
            listener = new PeerListener(pool, listenerHooks, new PeerListenerConfiguration());

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
        public void ShouldTriggerPeerConnectedOnLeft()
        {
            FileHash hash = FileHash.Random();

            var handler = leftHooks.OnPeerConnected.Trigger(data =>
            {
            });

            leftHooks.OnPeerConnected = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerPeerConnectedOnRight()
        {
            FileHash hash = FileHash.Random();

            var handler = rightHooks.OnPeerConnected.Trigger(data =>
            {
            });

            leftHooks.OnPeerConnected = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerPeerStateChangedWhenUnchoke()
        {
            FileHash hash = FileHash.Random();

            var leftHandler = leftHooks.OnPeerStateChanged.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.IsLocalChokingRemote.Should().BeTrue();
                data.IsLocalInterestedInRemote.Should().BeFalse();
                data.IsRemoteChokingLocal.Should().BeFalse();
                data.IsRemoteInterestedInLocal.Should().BeFalse();
            });

            var rightHandler = rightHooks.OnPeerStateChanged.Trigger(data =>
            {
                data.Peer.Should().Be(leftHash);
                data.IsLocalChokingRemote.Should().BeFalse();
                data.IsLocalInterestedInRemote.Should().BeFalse();
                data.IsRemoteChokingLocal.Should().BeTrue();
                data.IsRemoteInterestedInLocal.Should().BeFalse();
            });

            leftHooks.OnPeerStateChanged = leftHandler;
            rightHooks.OnPeerStateChanged = rightHandler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            rightConnected.Wait();
            right.SendUnchoke(leftHash);

            leftHandler.Wait().Should().BeTrue();
            rightHandler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerPeerStateChangedWhenChoke()
        {
            FileHash hash = FileHash.Random();

            var leftHandler = leftHooks.OnPeerStateChanged.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.IsLocalChokingRemote.Should().BeTrue();
                data.IsLocalInterestedInRemote.Should().BeFalse();
                data.IsRemoteChokingLocal.Should().BeTrue();
                data.IsRemoteInterestedInLocal.Should().BeFalse();
            });

            var rightHandler = rightHooks.OnPeerStateChanged.Trigger(data =>
            {
                data.Peer.Should().Be(leftHash);
                data.IsLocalChokingRemote.Should().BeTrue();
                data.IsLocalInterestedInRemote.Should().BeFalse();
                data.IsRemoteChokingLocal.Should().BeTrue();
                data.IsRemoteInterestedInLocal.Should().BeFalse();
            });

            leftHooks.OnPeerStateChanged = leftHandler;
            rightHooks.OnPeerStateChanged = rightHandler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            rightConnected.Wait();
            right.SendChoke(leftHash);

            leftHandler.Wait().Should().BeTrue();
            rightHandler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerPeerStateChangedWhenInterested()
        {
            FileHash hash = FileHash.Random();

            var leftHandler = leftHooks.OnPeerStateChanged.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.IsLocalChokingRemote.Should().BeTrue();
                data.IsLocalInterestedInRemote.Should().BeFalse();
                data.IsRemoteChokingLocal.Should().BeTrue();
                data.IsRemoteInterestedInLocal.Should().BeTrue();
            });

            var rightHandler = rightHooks.OnPeerStateChanged.Trigger(data =>
            {
                data.Peer.Should().Be(leftHash);
                data.IsLocalChokingRemote.Should().BeTrue();
                data.IsLocalInterestedInRemote.Should().BeTrue();
                data.IsRemoteChokingLocal.Should().BeTrue();
                data.IsRemoteInterestedInLocal.Should().BeFalse();
            });

            leftHooks.OnPeerStateChanged = leftHandler;
            rightHooks.OnPeerStateChanged = rightHandler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            rightConnected.Wait();
            right.SendInterested(leftHash);

            leftHandler.Wait().Should().BeTrue();
            rightHandler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerPeerBitfieldChangedWhenBitfield()
        {
            FileHash hash = FileHash.Random();
            Bitfield bitfield = Bitfield.Random(24);

            var handler = leftHooks.OnPeerBitfieldChanged.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.Bitfield.Should().Be(bitfield);
            });

            leftHooks.OnPeerBitfieldChanged = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            rightConnected.Wait();
            right.SendBitfield(leftHash, bitfield);

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerPeerBitfieldChangedWhenHave()
        {
            FileHash hash = FileHash.Random();
            Bitfield bitfield = new Bitfield(24);

            var handler = leftHooks.OnPeerBitfieldChanged.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.Bitfield.Should().NotBeNull();
                data.Bitfield[17].Should().BeTrue();
            });

            leftHooks.OnPeerBitfieldChanged = handler;

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            leftConnected.Wait();
            left.SetPieces(24);
            bitfield[17] = true;

            rightConnected.Wait();
            right.SendHave(leftHash, 17);

            handler.Wait().Should().BeTrue();
        }
    }
}