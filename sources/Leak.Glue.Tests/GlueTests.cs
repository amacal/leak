using System.Threading.Tasks;
using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Peer.Coordinator.Tests
{
    public class GlueTests
    {
        [Test]
        public async Task ShouldTriggerPeerConnected()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Left.Hooks.OnPeerConnected, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                });

                using (GlueInstance instance = session.Left.Build())
                {
                    instance.Service.Connect(session.Right.Connection, session.Right.Handshake);
                }

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerPeerStatusChangedWhenUnchoke()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger left = Trigger.Bind(ref session.Left.Hooks.OnPeerStatusChanged, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.State.IsLocalChokingRemote.Should().BeTrue();
                    data.State.IsLocalInterestedInRemote.Should().BeFalse();
                    data.State.IsRemoteChokingLocal.Should().BeFalse();
                    data.State.IsRemoteInterestedInLocal.Should().BeFalse();
                });

                Trigger right = Trigger.Bind(ref session.Right.Hooks.OnPeerStatusChanged, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.State.IsLocalChokingRemote.Should().BeFalse();
                    data.State.IsLocalInterestedInRemote.Should().BeFalse();
                    data.State.IsRemoteChokingLocal.Should().BeTrue();
                    data.State.IsRemoteInterestedInLocal.Should().BeFalse();
                });

                using (GlueInstance iLeft = session.Left.Build())
                using (GlueInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);
                    iRight.Service.SendUnchoke(session.Left.Peer);

                    left.Wait().Should().BeTrue();
                    right.Wait().Should().BeTrue();
                }
            }
        }

        [Test]
        public async Task ShouldTriggerPeerStatusChangedWhenInterested()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger left = Trigger.Bind(ref session.Left.Hooks.OnPeerStatusChanged, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.State.IsLocalChokingRemote.Should().BeTrue();
                    data.State.IsLocalInterestedInRemote.Should().BeFalse();
                    data.State.IsRemoteChokingLocal.Should().BeTrue();
                    data.State.IsRemoteInterestedInLocal.Should().BeTrue();
                });

                Trigger right = Trigger.Bind(ref session.Right.Hooks.OnPeerStatusChanged, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.State.IsLocalChokingRemote.Should().BeTrue();
                    data.State.IsLocalInterestedInRemote.Should().BeTrue();
                    data.State.IsRemoteChokingLocal.Should().BeTrue();
                    data.State.IsRemoteInterestedInLocal.Should().BeFalse();
                });

                using (GlueInstance iLeft = session.Left.Build())
                using (GlueInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);
                    iRight.Service.SendInterested(session.Left.Peer);
                }

                left.Wait().Should().BeTrue();
                right.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerPeerChangedWhenBitfield()
        {
            Bitfield bitfield = Bitfield.Random(24);

            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Left.Hooks.OnPeerBitfieldChanged, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Bitfield.Should().Be(bitfield);
                });

                using (GlueInstance iLeft = session.Left.Build())
                using (GlueInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);
                    iRight.Service.SendBitfield(session.Left.Peer, bitfield);
                }

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerPeerBitfieldChangedWhenHave()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Left.Hooks.OnPeerBitfieldChanged, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Bitfield.Should().NotBeNull();
                    data.Bitfield[1].Should().BeTrue();
                });

                using (GlueInstance iLeft = session.Left.Build())
                using (GlueInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    iLeft.Service.Handle(new MetafileVerified
                    {
                        Hash = session.Metainfo.Hash,
                        Metainfo = session.Metainfo,
                        Size = 232
                    });

                    iRight.Service.SendHave(session.Left.Peer, 1);
                }

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerExtensionListReceived()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger left = Trigger.Bind(ref session.Left.Hooks.OnExtensionListReceived, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Extensions.Should().BeEquivalentTo("right-a");
                });

                Trigger right = Trigger.Bind(ref session.Right.Hooks.OnExtensionListReceived, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.Extensions.Should().BeEquivalentTo("left-a", "left-b");
                });

                using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
                using (GlueInstance iRight = session.Right.Build("right-a"))
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);
                }

                left.Wait().Should().BeTrue();
                right.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerExtensionListSent()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger left = Trigger.Bind(ref session.Left.Hooks.OnExtensionListSent, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Extensions.Should().BeEquivalentTo("left-a", "left-b");
                });

                Trigger right = Trigger.Bind(ref session.Right.Hooks.OnExtensionListSent, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.Extensions.Should().BeEquivalentTo("right-a");
                });

                using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
                using (GlueInstance iRight = session.Right.Build("right-a"))
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);
                }

                left.Wait().Should().BeTrue();
                right.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldFindOutIfExtensionIsSupported()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger left = Trigger.Bind(ref session.Left.Hooks.OnExtensionListSent);
                Trigger right = Trigger.Bind(ref session.Right.Hooks.OnExtensionListSent);

                using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
                using (GlueInstance iRight = session.Right.Build("right-a"))
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    left.Wait().Should().BeTrue();
                    right.Wait().Should().BeTrue();

                    iLeft.Service.IsSupported(session.Right.Peer, "right-a").Should().BeTrue();
                    iLeft.Service.IsSupported(session.Right.Peer, "right-b").Should().BeFalse();
                }
            }
        }

        [Test]
        public async Task ShouldTriggerExtensionDataReceived()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger received = Trigger.Bind(ref session.Right.Hooks.OnExtensionListReceived);
                Trigger handler = Trigger.Bind(ref session.Left.Hooks.OnExtensionDataReceived, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Extension.Should().Be("left-a");
                    data.Size.Should().Be(10);
                });

                using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
                using (GlueInstance iRight = session.Right.Build("right-a", "left-a"))
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    received.Wait().Should().BeTrue();
                    iRight.Service.SendExtension(session.Left.Peer, "left-a", new byte[10]);
                }

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerExtensionDataSent()
        {
            using (GlueFixture fixture = new GlueFixture())
            using (GlueSession session = await fixture.Start())
            {
                Trigger received = Trigger.Bind(ref session.Right.Hooks.OnExtensionListReceived);
                Trigger handler = Trigger.Bind(ref session.Right.Hooks.OnExtensionDataSent, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.Extension.Should().Be("left-a");
                    data.Size.Should().Be(10);
                });

                using (GlueInstance iLeft = session.Left.Build("left-a", "left-b"))
                using (GlueInstance iRight = session.Right.Build("right-a", "left-a"))
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    received.Wait().Should().BeTrue();
                    iRight.Service.SendExtension(session.Left.Peer, "left-a", new byte[10]);
                }

                handler.Wait().Should().BeTrue();
            }
        }
    }
}