using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Leakage.Tests
{
    public class LeakageTests
    {
        [Test]
        public void ShouldStartListener()
        {
            using (LeakageFixture fixture = new LeakageFixture())
            using (LeakageSwarm swarm = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref swarm.Sue.Hooks.OnListenerStarted, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Port.Should().BeGreaterThan(0);
                });

                swarm.Sue.Client.Start();
                swarm.Sue.Client.Register(swarm.Sue.Registrant);

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldConnectToBob()
        {
            using (LeakageFixture fixture = new LeakageFixture())
            using (LeakageSwarm swarm = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref swarm.Sue.Hooks.OnPeerConnected, data =>
                {
                    data.Peer.Should().Be(swarm.Bob.Client.Peer);
                });

                swarm.Bob.Client.Start();
                swarm.Bob.Client.Register(swarm.Bob.Registrant);
                swarm.Bob.Events.Listening.Wait(5000).Should().BeTrue();

                swarm.Sue.Client.Start();
                swarm.Sue.Client.Register(swarm.Sue.Registrant.With(swarm.Bob.Address));

                trigger.Wait().Should().BeTrue();
            }
        }
    }
}