using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;
using System.Net;
using FakeItEasy;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Receiver.Events;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersTests
    {
        [Test]
        public void ShouldTriggerPeersDataWhenLocal()
        {
            NetworkAddress remote = NetworkAddress.Parse(IPAddress.Loopback, 8345);

            using (PeersFixture fixture = new PeersFixture())
            using (PeersSession session = fixture.Start())
            {
                NetworkConnection connection = A.Fake<NetworkConnection>();
                Handshake handshake = new Handshake(PeerHash.Random(), PeerHash.Random(), session.Coordinator.Hash, HandshakeOptions.Extended);

                MessageReceived extended = new MessageReceived
                {
                    Type = "extended",
                    Peer = handshake.Remote,
                    Payload = new PeersMessage("d1:md6:ut_pexi3eee")
                };

                Trigger handler = Trigger.Bind(ref session.Plugin.Hooks.OnPeersDataSent, data =>
                {
                    data.Peer.Should().Be(handshake.Remote);
                    data.Hash.Should().Be(session.Coordinator.Hash);
                    data.Remotes.Should().Contain(remote);
                });

                session.Coordinator.Start();
                session.Coordinator.Connect(connection, handshake);

                session.Coordinator.Handle(extended);
                session.Coordinator.SendPeers(handshake.Remote, remote);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}