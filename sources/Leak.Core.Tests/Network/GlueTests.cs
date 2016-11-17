﻿using FluentAssertions;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
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
        private GlueConfiguration leftConfiguration;
        private GlueService right;
        private GlueHooks rightHooks;
        private GlueConfiguration rightConfiguration;
        private PeerHash rightHash;

        private Trigger<HandshakeCompleted> leftConnected;
        private Trigger<HandshakeCompleted> rightConnected;

        [SetUp]
        public void SetUp()
        {
            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            leftConfiguration = new GlueConfiguration();
            rightConfiguration = new GlueConfiguration();

            NetworkPool pool = new NetworkPool(worker, new NetworkPoolHooks());
            GlueFactory leftFactory = new GlueFactory(new BufferedBlockFactory(), leftConfiguration);
            GlueFactory rightFactory = new GlueFactory(new BufferedBlockFactory(), rightConfiguration);

            PeerListenerHooks listenerHooks = new PeerListenerHooks
            {
                OnHandshakeCompleted = rightConnected = new Trigger<HandshakeCompleted>(data =>
                {
                    leftHash = data.Handshake.Remote;
                    right = rightFactory.Create(data.Handshake.Hash, rightHooks);
                    right.Connect(data.Connection, data.Handshake);
                })
            };

            PeerConnectorHooks connectorHooks = new PeerConnectorHooks
            {
                OnHandshakeCompleted = leftConnected = new Trigger<HandshakeCompleted>(data =>
                {
                    rightHash = data.Handshake.Remote;
                    left = leftFactory.Create(data.Handshake.Hash, leftHooks);
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

        [Test]
        public void ShouldTriggerExtensionListReceived()
        {
            FileHash hash = FileHash.Random();

            var leftHandler = leftHooks.OnExtensionListReceived.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.Extensions.Should().BeEquivalentTo("right-c");
            });

            var rightHandler = rightHooks.OnExtensionListReceived.Trigger(data =>
            {
                data.Peer.Should().Be(leftHash);
                data.Extensions.Should().BeEquivalentTo("left-a", "left-b");
            });

            leftHooks.OnExtensionListReceived = leftHandler;
            rightHooks.OnExtensionListReceived = rightHandler;

            leftConfiguration.Plugins.Add(new Plugin("left-a"));
            leftConfiguration.Plugins.Add(new Plugin("left-b"));
            rightConfiguration.Plugins.Add(new Plugin("right-c"));

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            leftHandler.Wait().Should().BeTrue();
            rightHandler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerExtensionListSent()
        {
            FileHash hash = FileHash.Random();

            var leftHandler = leftHooks.OnExtensionListSent.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.Extensions.Should().BeEquivalentTo("left-a", "left-b");
            });

            var rightHandler = rightHooks.OnExtensionListSent.Trigger(data =>
            {
                data.Peer.Should().Be(leftHash);
                data.Extensions.Should().BeEquivalentTo("right-c");
            });

            leftHooks.OnExtensionListSent = leftHandler;
            rightHooks.OnExtensionListSent = rightHandler;

            leftConfiguration.Plugins.Add(new Plugin("left-a"));
            leftConfiguration.Plugins.Add(new Plugin("left-b"));
            rightConfiguration.Plugins.Add(new Plugin("right-c"));

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            leftHandler.Wait().Should().BeTrue();
            rightHandler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldFindOutIfExtensionIsSupported()
        {
            FileHash hash = FileHash.Random();

            var handler = leftHooks.OnExtensionListReceived.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
            });

            leftHooks.OnExtensionListReceived = handler;
            rightConfiguration.Plugins.Add(new Plugin("right-a"));

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            handler.Wait().Should().BeTrue();

            left.IsSupported(rightHash, "right-a").Should().BeTrue();
            left.IsSupported(rightHash, "right-b").Should().BeFalse();
        }

        [Test]
        public void ShouldTriggerExtensionDataReceived()
        {
            FileHash hash = FileHash.Random();

            var extended = leftHooks.OnExtensionListReceived.Trigger();
            var handler = leftHooks.OnExtensionDataReceived.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.Extension.Should().Be("left-a");
                data.Size.Should().Be(10);
            });

            leftHooks.OnExtensionListReceived = extended;
            leftHooks.OnExtensionDataReceived = handler;
            leftConfiguration.Plugins.Add(new Plugin("left-a"));

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            extended.Wait();
            right.SendExtension(leftHash, "left-a", new byte[10]);

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerExtensionDataSent()
        {
            FileHash hash = FileHash.Random();

            var extended = rightHooks.OnExtensionListSent.Trigger();
            var handler = rightHooks.OnExtensionDataSent.Trigger(data =>
            {
                data.Peer.Should().Be(rightHash);
                data.Extension.Should().Be("left-a");
                data.Size.Should().Be(10);
            });

            rightHooks.OnExtensionDataSent = handler;
            leftConfiguration.Plugins.Add(new Plugin("left-a"));

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            extended.Wait();
            right.SendExtension(leftHash, "left-a", new byte[10]);

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetadataRequestReceived()
        {
            FileHash hash = FileHash.Random();
            MetadataHooks metahooks = new MetadataHooks();
            MetadataPlugin plugin = new MetadataPlugin(metahooks);

            var extended = rightHooks.OnExtensionListSent.Trigger();
            var handler = metahooks.OnMetadataRequestReceived.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Peer.Should().Be(rightHash);
                data.Piece.Should().Be(7);
            });

            metahooks.OnMetadataRequestReceived = handler;
            leftConfiguration.Plugins.Add(plugin);

            listener.Enable(hash);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", 8080));

            extended.Wait();
            right.SendMetadataRequest(leftHash, 7);

            handler.Wait().Should().BeTrue();
        }

        private class Plugin : GluePlugin, GlueHandler
        {
            private readonly string name;

            public Plugin(string name)
            {
                this.name = name;
            }

            public void Install(GlueMore more)
            {
                more.Add(name, this);
            }

            public void Handle(FileHash hash, PeerHash peer, byte[] payload)
            {
            }
        }
    }
}