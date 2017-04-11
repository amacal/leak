using System.Threading.Tasks;
using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Peer.Receiver.Tests
{
    public class LoopyTests
    {
        [Test]
        public async Task ShouldTriggerMessageReceivedWhenKeepAlive()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("keep-alive");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.KeepAlive, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedWhenChoke()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("choke");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Choke, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedWhenUnchoke()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("unchoke");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Unchoke, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedWhenInterested()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("interested");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Interested, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedWhenHave()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("have");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Have, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedWhenBitfield()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("bitfield");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Bitfield, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedWhenRequest()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("request");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Request, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedWhenPiece()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("piece");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Piece, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceivedExtended()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("extended");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Extended, null);
                handler.Wait().Should().BeTrue();
            }
        }
    }
}