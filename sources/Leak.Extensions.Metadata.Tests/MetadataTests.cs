using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataTests
    {
        [Test]
        public async Task ShouldTriggerMetadataRequest()
        {
            using (MetadataFixture fixture = new MetadataFixture())
            using (MetadataSession session = await fixture.Start())
            {
                Trigger received = Trigger.Bind(ref session.Left.Hooks.OnExtensionListReceived);
                Trigger left = Trigger.Bind(ref session.Left.Metadata.OnMetadataRequestReceived, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(7);
                });

                Trigger right = Trigger.Bind(ref session.Right.Metadata.OnMetadataRequestSent, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(7);
                });

                using (MetadataInstance iLeft = session.Left.Build())
                using (MetadataInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    received.Wait().Should().BeTrue();
                    iRight.Service.SendMetadataRequest(session.Left.Peer, 7);
                }

                left.Wait().Should().BeTrue();
                right.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMetadataReject()
        {
            using (MetadataFixture fixture = new MetadataFixture())
            using (MetadataSession session = await fixture.Start())
            {
                Trigger received = Trigger.Bind(ref session.Left.Hooks.OnExtensionListReceived);
                Trigger left = Trigger.Bind(ref session.Left.Metadata.OnMetadataRejectReceived, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(7);
                });

                Trigger right = Trigger.Bind(ref session.Right.Metadata.OnMetadataRejectSent, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(7);
                });

                using (MetadataInstance iLeft = session.Left.Build())
                using (MetadataInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    received.Wait().Should().BeTrue();
                    iRight.Service.SendMetadataReject(session.Left.Peer, 7);
                }

                left.Wait().Should().BeTrue();
                right.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMetadataReceive()
        {
            using (MetadataFixture fixture = new MetadataFixture())
            using (MetadataSession session = await fixture.Start())
            {
                Trigger received = Trigger.Bind(ref session.Left.Hooks.OnExtensionListReceived);
                Trigger left = Trigger.Bind(ref session.Left.Metadata.OnMetadataPieceReceived, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(7);
                    data.Data.Should().NotBeNull();
                    data.Data.Length.Should().Be(1023);
                });

                Trigger right = Trigger.Bind(ref session.Right.Metadata.OnMetadataPieceSent, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(7);
                    data.Data.Should().NotBeNull();
                    data.Data.Length.Should().Be(1023);
                });

                using (MetadataInstance iLeft = session.Left.Build())
                using (MetadataInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    received.Wait().Should().BeTrue();
                    iRight.Service.SendMetadataPiece(session.Left.Peer, 7, 128, new byte[1023]);
                }

                left.Wait().Should().BeTrue();
                right.Wait().Should().BeTrue();
            }
        }
    }
}