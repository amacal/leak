using System.Threading.Tasks;
using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Peer.Receiver.Tests
{
    public class LoopTests
    {
        [Test]
        public async Task ShouldTriggerKeepAlive()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnKeepAliveReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.KeepAlive, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageReceived()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageReceived, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("found");
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Known, null);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageIgnored()
        {
            using (LoopFixture fixture = new LoopFixture())
            using (LoopSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageIgnored, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Identifier.Should().Be(0x02);
                    data.Payload.Should().NotBeNull();
                });

                session.Client.Send(fixture.Samples.Unknown, null);
                handler.Wait().Should().BeTrue();
            }
        }
    }
}