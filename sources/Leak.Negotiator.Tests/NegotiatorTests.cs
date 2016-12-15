using System.Threading.Tasks;
using FluentAssertions;
using Leak.Common;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Negotiator.Tests
{
    public class NegotiatorTests
    {
        [Test]
        public async Task ShouldTriggersHandshakeCompletedOnHandshakeInitiator()
        {
            FileHash hash = FileHash.Random();

            using (NegotiatorFixture fixture = new NegotiatorFixture())
            using (NegotiatorFixturePair pair = await fixture.Create())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnHandshakeCompleted, data =>
                {
                    data.Connection.Should().NotBeNull();

                    if (data.Connection.Identifier == pair.Local.Identifier)
                    {
                        data.Hash.Should().Be(hash);
                        data.Handshake.Should().NotBeNull();
                    }

                    return data.Connection.Identifier == pair.Local.Identifier;
                });

                fixture.Negotiator.Start(pair.Local, hash);
                fixture.Negotiator.Handle(pair.Remote, hash);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggersHandshakeCompletedOnHandshakeReceiver()
        {
            FileHash hash = FileHash.Random();

            using (NegotiatorFixture fixture = new NegotiatorFixture())
            using (NegotiatorFixturePair pair = await fixture.Create())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnHandshakeCompleted, data =>
                {
                    data.Connection.Should().NotBeNull();

                    if (data.Connection.Identifier == pair.Remote.Identifier)
                    {
                        data.Hash.Should().Be(hash);
                        data.Handshake.Should().NotBeNull();
                    }

                    return data.Connection.Identifier == pair.Remote.Identifier;
                });

                fixture.Negotiator.Start(pair.Local, hash);
                fixture.Negotiator.Handle(pair.Remote, hash);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}
