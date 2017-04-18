using FakeItEasy;
using FluentAssertions;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Receiver.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorTests
    {
        [Test]
        public void ShouldTriggerPeerConnected()
        {
            using (CoordinatorFixture fixture = new CoordinatorFixture())
            using (CoordinatorSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                Trigger handler = Trigger.Bind(ref session.Coordinator.Hooks.OnPeerConnected, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.Connection.Should().Be(connection);
                });

                session.Coordinator.Connect(connection, handshake);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPeerDisconnected()
        {
            using (CoordinatorFixture fixture = new CoordinatorFixture())
            using (CoordinatorSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                Trigger handler = Trigger.Bind(ref session.Coordinator.Hooks.OnPeerDisconnected, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Disconnect(connection);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerStatusChangedWhenLocalUnchoked()
        {
            using (CoordinatorFixture fixture = new CoordinatorFixture())
            using (CoordinatorSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                Trigger handler = Trigger.Bind(ref session.Coordinator.Hooks.OnStatusChanged, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.State.IsLocalChokingRemote.Should().BeFalse();
                    data.State.IsLocalInterestedInRemote.Should().BeFalse();
                    data.State.IsRemoteChokingLocal.Should().BeTrue();
                    data.State.IsRemoteInterestedInLocal.Should().BeFalse();
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Unchoke(handshake.Remote);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerStatusChangedWhenRemoteUnchoked()
        {
            using (CoordinatorFixture fixture = new CoordinatorFixture())
            using (CoordinatorSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                MessageReceived received = new MessageReceived
                {
                    Type = "unchoke",
                    Peer = handshake.Remote,
                    Payload = new CoordinatorMessage(0x00, 0x00, 0x00, 0x01, 0x01)
                };

                Trigger handler = Trigger.Bind(ref session.Coordinator.Hooks.OnStatusChanged, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.State.IsLocalChokingRemote.Should().BeTrue();
                    data.State.IsLocalInterestedInRemote.Should().BeFalse();
                    data.State.IsRemoteChokingLocal.Should().BeFalse();
                    data.State.IsRemoteInterestedInLocal.Should().BeFalse();
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Handle(received);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPeerStatusChangedWhenLocalInterested()
        {
            using (CoordinatorFixture fixture = new CoordinatorFixture())
            using (CoordinatorSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                Trigger handler = Trigger.Bind(ref session.Coordinator.Hooks.OnStatusChanged, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.State.IsLocalChokingRemote.Should().BeTrue();
                    data.State.IsLocalInterestedInRemote.Should().BeTrue();
                    data.State.IsRemoteChokingLocal.Should().BeTrue();
                    data.State.IsRemoteInterestedInLocal.Should().BeFalse();
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Interested(handshake.Remote);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPeerStatusChangedWhenRemoteInterested()
        {
            using (CoordinatorFixture fixture = new CoordinatorFixture())
            using (CoordinatorSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                MessageReceived received = new MessageReceived
                {
                    Type = "interested",
                    Peer = handshake.Remote,
                    Payload = new CoordinatorMessage(0x00, 0x00, 0x00, 0x01, 0x02)
                };

                Trigger handler = Trigger.Bind(ref session.Coordinator.Hooks.OnStatusChanged, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.State.IsLocalChokingRemote.Should().BeTrue();
                    data.State.IsLocalInterestedInRemote.Should().BeFalse();
                    data.State.IsRemoteChokingLocal.Should().BeTrue();
                    data.State.IsRemoteInterestedInLocal.Should().BeTrue();
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Handle(received);

                handler.Wait().Should().BeTrue();
            }
        }

        //[Test]
        //public async Task ShouldTriggerExtensionListReceived()
        //{
        //    using (GlueFixture fixture = new GlueFixture())
        //    using (GlueSession session = await fixture.Start())
        //    {
        //        Trigger left = Trigger.Bind(ref session.Left.Hooks.OnExtensionListReceived, data =>
        //        {
        //            data.Peer.Should().Be(session.Right.Peer);
        //            data.Extensions.Should().BeEquivalentTo("right-a");
        //        });

        //        Trigger right = Trigger.Bind(ref session.Right.Hooks.OnExtensionListReceived, data =>
        //        {
        //            data.Peer.Should().Be(session.Left.Peer);
        //            data.Extensions.Should().BeEquivalentTo("left-a", "left-b");
        //        });

        //        using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
        //        using (GlueInstance iRight = session.Right.Build("right-a"))
        //        {
        //            iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
        //            iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);
        //        }

        //        left.Wait().Should().BeTrue();
        //        right.Wait().Should().BeTrue();
        //    }
        //}

        //[Test]
        //public async Task ShouldTriggerExtensionListSent()
        //{
        //    using (GlueFixture fixture = new GlueFixture())
        //    using (GlueSession session = await fixture.Start())
        //    {
        //        Trigger left = Trigger.Bind(ref session.Left.Hooks.OnExtensionListSent, data =>
        //        {
        //            data.Peer.Should().Be(session.Right.Peer);
        //            data.Extensions.Should().BeEquivalentTo("left-a", "left-b");
        //        });

        //        Trigger right = Trigger.Bind(ref session.Right.Hooks.OnExtensionListSent, data =>
        //        {
        //            data.Peer.Should().Be(session.Left.Peer);
        //            data.Extensions.Should().BeEquivalentTo("right-a");
        //        });

        //        using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
        //        using (GlueInstance iRight = session.Right.Build("right-a"))
        //        {
        //            iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
        //            iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);
        //        }

        //        left.Wait().Should().BeTrue();
        //        right.Wait().Should().BeTrue();
        //    }
        //}

        //[Test]
        //public async Task ShouldFindOutIfExtensionIsSupported()
        //{
        //    using (GlueFixture fixture = new GlueFixture())
        //    using (GlueSession session = await fixture.Start())
        //    {
        //        Trigger left = Trigger.Bind(ref session.Left.Hooks.OnExtensionListSent);
        //        Trigger right = Trigger.Bind(ref session.Right.Hooks.OnExtensionListSent);

        //        using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
        //        using (GlueInstance iRight = session.Right.Build("right-a"))
        //        {
        //            iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
        //            iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

        //            left.Wait().Should().BeTrue();
        //            right.Wait().Should().BeTrue();

        //            iLeft.Service.IsSupported(session.Right.Peer, "right-a").Should().BeTrue();
        //            iLeft.Service.IsSupported(session.Right.Peer, "right-b").Should().BeFalse();
        //        }
        //    }
        //}

        //[Test]
        //public async Task ShouldTriggerExtensionDataReceived()
        //{
        //    using (GlueFixture fixture = new GlueFixture())
        //    using (GlueSession session = await fixture.Start())
        //    {
        //        Trigger received = Trigger.Bind(ref session.Right.Hooks.OnExtensionListReceived);
        //        Trigger handler = Trigger.Bind(ref session.Left.Hooks.OnExtensionDataReceived, data =>
        //        {
        //            data.Peer.Should().Be(session.Right.Peer);
        //            data.Extension.Should().Be("left-a");
        //            data.Size.Should().Be(10);
        //        });

        //        using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
        //        using (GlueInstance iRight = session.Right.Build("right-a", "left-a"))
        //        {
        //            iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
        //            iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

        //            received.Wait().Should().BeTrue();
        //            iRight.Service.SendExtension(session.Left.Peer, "left-a", new byte[10]);
        //        }

        //        handler.Wait().Should().BeTrue();
        //    }
        //}

        //[Test]
        //public async Task ShouldTriggerExtensionDataSent()
        //{
        //    using (GlueFixture fixture = new GlueFixture())
        //    using (GlueSession session = await fixture.Start())
        //    {
        //        Trigger received = Trigger.Bind(ref session.Right.Hooks.OnExtensionListReceived);
        //        Trigger handler = Trigger.Bind(ref session.Right.Hooks.OnExtensionDataSent, data =>
        //        {
        //            data.Peer.Should().Be(session.Left.Peer);
        //            data.Extension.Should().Be("left-a");
        //            data.Size.Should().Be(10);
        //        });

        //        using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
        //        using (GlueInstance iRight = session.Right.Build("right-a", "left-a"))
        //        {
        //            iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
        //            iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

        //            received.Wait().Should().BeTrue();
        //            iRight.Service.SendExtension(session.Left.Peer, "left-a", new byte[10]);
        //        }

        //        handler.Wait().Should().BeTrue();
        //    }
        //}
    }
}