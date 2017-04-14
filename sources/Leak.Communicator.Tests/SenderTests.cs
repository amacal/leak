using System.Threading.Tasks;
using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Peer.Sender.Tests
{
    public class SenderTests
    {
        [Test]
        public async Task ShouldTriggerKeepAlive()
        {
            using (SenderFixture fixture = new SenderFixture())
            using (SenderSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnKeepAliveSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                });

                session.Communicator.SendKeepAlive();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageSent()
        {
            using (SenderFixture fixture = new SenderFixture())
            using (SenderSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("found");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.Send(new SenderFound());
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerMessageIgnored()
        {
            using (SenderFixture fixture = new SenderFixture())
            using (SenderSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnMessageIgnored, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("unknown");
                    data.Payload.Should().NotBeNull();
                });

                session.Communicator.Send(new SenderUnknown());
                handler.Wait().Should().BeTrue();
            }
        }
    }
}