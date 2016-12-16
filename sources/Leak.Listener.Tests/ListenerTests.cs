using System.Threading.Tasks;
using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Listener.Tests
{
    public class ListenerTests
    {
        [Test]
        public async Task ShouldTriggerListenerStarted()
        {
            using (ListenerFixture fixture = new ListenerFixture())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnListenerStarted, data =>
                {
                    data.Port.Should().BeGreaterThan(0);
                    data.Peer.Should().NotBeNull();
                });

                await fixture.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public async Task ShouldTriggerOnConnectionArrived()
        {
            using (ListenerFixture fixture = new ListenerFixture())
            using (ListenerSession session = await fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref fixture.Hooks.OnConnectionArrived, data =>
                {
                    data.Remote.Should().NotBeNull();
                    data.Remote.Host.Should().Be("127.0.0.1");
                    data.Remote.Port.Should().BeGreaterThan(0);
                    data.Remote.Port.Should().NotBe(session.Endpoint.Port);
                });

                session.Client.Connect(session.Endpoint, null);
                handler.Wait().Should().BeTrue();
            }
        }
    }
}