using FluentAssertions;
using Leak.Common;
using Leak.Testing;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersTests
    {
        [Test]
        public async Task ShouldTriggerPeersData()
        {
            PeerAddress remote = PeerAddress.Parse(IPAddress.Loopback, 8345);

            using (PeersFixture fixture = new PeersFixture())
            using (PeersSession session = await fixture.Start())
            {
                Trigger received = Trigger.Bind(ref session.Left.Hooks.OnExtensionListReceived);
                Trigger left = Trigger.Bind(ref session.Left.Peers.OnPeersDataReceived, data =>
                {
                    data.Peer.Should().Be(session.Right.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Remotes.Should().Contain(remote);
                });

                Trigger right = Trigger.Bind(ref session.Right.Peers.OnPeersDataSent, data =>
                {
                    data.Peer.Should().Be(session.Left.Peer);
                    data.Hash.Should().Be(session.Hash);
                    data.Remotes.Should().Contain(remote);
                });

                using (PeersInstance iLeft = session.Left.Build())
                using (PeersInstance iRight = session.Right.Build())
                {
                    iLeft.Service.Connect(session.Right.Connection, session.Right.Handshake);
                    iRight.Service.Connect(session.Left.Connection, session.Left.Handshake);

                    received.Wait().Should().BeTrue();
                    iRight.Service.SendPeers(session.Left.Peer, remote);
                }

                left.Wait().Should().BeTrue();
                right.Wait().Should().BeTrue();
            }
        }
    }
}