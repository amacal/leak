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

        [Test]
        public void ShouldReceiveExchangedPeer()
        {
            using (LeakageFixture fixture = new LeakageFixture())
            using (LeakageSwarm swarm = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref swarm.Joe.Hooks.OnPeerListReceived, data =>
                {
                    data.Peer.Should().Be(swarm.Sue.Client.Peer);
                    data.Hash.Should().NotBeNull();
                    data.Remotes.Should().Contain(swarm.Bob.Address);
                });

                swarm.Bob.Client.Start();
                swarm.Bob.Client.Register(swarm.Bob.Registrant);
                swarm.Bob.Events.Listening.Wait(5000).Should().BeTrue();

                swarm.Sue.Client.Start();
                swarm.Sue.Client.Register(swarm.Sue.Registrant.With(swarm.Bob.Address));
                swarm.Sue.Events.Connected.Wait(5000).Should().BeTrue();

                swarm.Joe.Client.Start();
                swarm.Joe.Client.Register(swarm.Joe.Registrant.With(swarm.Sue.Address));
                swarm.Joe.Events.Connected.Wait(5000).Should().BeTrue();

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldConnectedToExchangedPeer()
        {
            using (LeakageFixture fixture = new LeakageFixture())
            using (LeakageSwarm swarm = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref swarm.Joe.Hooks.OnPeerConnected, data =>
                {
                    return data.Peer.Equals(swarm.Bob.Client.Peer);
                });

                swarm.Bob.Client.Start();
                swarm.Bob.Client.Register(swarm.Bob.Registrant);
                swarm.Bob.Events.Listening.Wait(5000).Should().BeTrue();

                swarm.Sue.Client.Start();
                swarm.Sue.Client.Register(swarm.Sue.Registrant.With(swarm.Bob.Address));
                swarm.Sue.Events.Connected.Wait(5000).Should().BeTrue();

                swarm.Joe.Client.Start();
                swarm.Joe.Client.Register(swarm.Joe.Registrant.With(swarm.Sue.Address));
                swarm.Joe.Events.Connected.Wait(5000).Should().BeTrue();

                trigger.Wait().Should().BeTrue();
            }
        }
    }
}