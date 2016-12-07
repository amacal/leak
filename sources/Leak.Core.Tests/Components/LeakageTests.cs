using FluentAssertions;
using Leak.Common;
using Leak.Core.Leakage;
using Leak.Core.Tests.Core;
using NUnit.Framework;

namespace Leak.Core.Tests.Components
{
    public class LeakageTests
    {
        private LeakHooks hooks;

        [SetUp]
        public void SetUp()
        {
            hooks = new LeakHooks();
        }

        [Test]
        public void ShouldStartListener()
        {
            LeakConfiguration configuration = new LeakConfiguration
            {
                Port = LeakPort.Random,
                Peer = PeerHash.Random()
            };

            Trigger trigger = Trigger.Bind(ref hooks.OnListenerStarted, data =>
            {
                data.Port.Should().BeGreaterThan(0);
                data.Peer.Should().Be(configuration.Peer);
            });

            using (new LeakClient(hooks, configuration))
            {
                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldDealWithoutListener()
        {
            LeakConfiguration configuration = new LeakConfiguration
            {
            };

            using (LeakClient client = new LeakClient(hooks, configuration))
            {
            }
        }

        [Test]
        public void ShouldConnectToSue()
        {
            using (Sue sue = new Sue())
            {
                LeakConfiguration configuration = new LeakConfiguration
                {
                };

                Trigger trigger = Trigger.Bind(ref hooks.OnPeerConnected, data =>
                {
                    data.Peer.Should().Be(sue.Peer);
                });

                LeakRegistrant registrant = new LeakRegistrant
                {
                    Hash = sue.Hash,
                    Peers = new[] { sue.Endpoint }
                };

                using (LeakClient client = new LeakClient(hooks, configuration))
                {
                    client.Register(registrant);
                    trigger.Wait().Should().BeTrue();
                }
            }
        }
    }
}