using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;
using FakeItEasy;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Receiver.Events;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataTests
    {
        // ShouldTriggerMessageIgnoredWhenSpecificExtensionNotSupported
        // ShouldTriggerMessageIgnoredWhenAllExtensionsDisabled

        [Test]
        public void ShouldTriggerMetadataRequestedWhenLocal()
        {
            using (MetadataFixture fixture = new MetadataFixture())
            using (MetadataSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.Extended);

                MessageReceived extended = new MessageReceived
                {
                    Type = "extended",
                    Peer = handshake.Remote,
                    Payload = new MetadataMessage("d1:md11:ut_metadatai3ee13:metadata_sizei31235ee")
                };

                Trigger handler = Trigger.Bind(ref session.Plugin.Hooks.OnMetadataRequestSent, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.Hash.Should().Be(session.Coordinator.Hash);
                    data.Piece.Should().Be(7);
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Handle(extended);

                session.Coordinator.SendMetadataRequest(handshake.Remote, 7);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataRejectWhenLocal()
        {
            using (MetadataFixture fixture = new MetadataFixture())
            using (MetadataSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                MessageReceived extended = new MessageReceived
                {
                    Type = "extended",
                    Peer = handshake.Remote,
                    Payload = new MetadataMessage("d1:md11:ut_metadatai3ee13:metadata_sizei31235ee")
                };

                Trigger handler = Trigger.Bind(ref session.Plugin.Hooks.OnMetadataRejectSent, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.Hash.Should().Be(session.Coordinator.Hash);
                    data.Piece.Should().Be(7);
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Handle(extended);

                session.Coordinator.SendMetadataReject(handshake.Remote, 7);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataReceiveWhenLocal()
        {
            using (MetadataFixture fixture = new MetadataFixture())
            using (MetadataSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.None);

                MessageReceived extended = new MessageReceived
                {
                    Type = "extended",
                    Peer = handshake.Remote,
                    Payload = new MetadataMessage("d1:md11:ut_metadatai3ee13:metadata_sizei31235ee")
                };

                Trigger handler = Trigger.Bind(ref session.Plugin.Hooks.OnMetadataPieceSent, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.Hash.Should().Be(session.Coordinator.Hash);
                    data.Piece.Should().Be(7);
                    data.Data?.Length.Should().Be(1023);
                });

                session.Coordinator.Connect(connection, handshake);
                session.Coordinator.Handle(extended);

                session.Coordinator.SendMetadataPiece(handshake.Remote, 7, 6 * 16384 + 1023, new byte[1023]);
                handler.Wait().Should().BeTrue();
            }
        }
    }
}