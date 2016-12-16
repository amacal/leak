using System.Threading.Tasks;
using FluentAssertions;
using Leak.Common;
using Leak.Communicator.Messages;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Communicator.Tests
{
    public class CommunicatorTests
    {
        [Test]
        public async Task ShouldTriggerMessageSentWhenKeepAlive()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("keep-alive");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendKeepAlive();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSentWhenChoke()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("choke");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendChoke();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSentWhenUnchoke()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("unchoke");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendUnchoke();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSentWhenInterested()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("interested");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendInterested();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSentWhenHave()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("have");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendHave(2);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSentWhenBitfield()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("bitfield");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendBitfield(new Bitfield(20));
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSentWhenPiece()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("piece");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendPiece(new Piece(1, 2, new FixedDataBlock(new byte[2])));
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSentWhenExtended()
        {
            using (CommunicatorFixture fixture = new CommunicatorFixture())
            using (CommunicatorSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("extended");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.SendExtended(new Extended(17, new byte[2]));
                handler.Wait().Should().BeTrue();
            }
        }
    }
}