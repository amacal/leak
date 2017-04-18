using FakeItEasy;
using FluentAssertions;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Peer.Sender.Tests
{
    public class SenderTests
    {
        [Test]
        public void ShouldTriggerKeepAliveSent()
        {
            using (SenderFixture fixture = new SenderFixture())
            using (SenderSession session = fixture.Start())
            {
                PeerHash peer = PeerHash.Random();
                NetworkConnection connection = A.Fake<NetworkConnection>();

                Trigger handler = Trigger.Bind(ref session.Hooks.OnKeepAliveSent, data =>
                {
                    data.Peer.Should().Be(peer);
                });

                session.Sender.Add(peer, connection);
                session.Sender.SendKeepAlive(peer);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMessageSent()
        {
            using (SenderFixture fixture = new SenderFixture())
            using (SenderSession session = fixture.Start())
            {
                PeerHash peer = PeerHash.Random();
                NetworkConnection connection = A.Fake<NetworkConnection>();

                Trigger handler = Trigger.Bind(ref session.Hooks.OnMessageSent, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("found");
                    data.Payload.Should().NotBeNull();
                });

                session.Sender.Add(peer, connection);
                session.Sender.Send(peer, new SenderFound());

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMessageIgnoredWhenMessageNotMapped()
        {
            using (SenderFixture fixture = new SenderFixture())
            using (SenderSession session = fixture.Start())
            {
                PeerHash peer = PeerHash.Random();
                NetworkConnection connection = A.Fake<NetworkConnection>();

                Trigger handler = Trigger.Bind(ref session.Hooks.OnMessageIgnored, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("unknown");
                    data.Payload.Should().NotBeNull();
                });

                session.Sender.Add(peer, connection);
                session.Sender.Send(peer, new SenderUnknown());

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMessageIgnoredWhenPeerNotFound()
        {
            using (SenderFixture fixture = new SenderFixture())
            using (SenderSession session = fixture.Start())
            {
                PeerHash peer = PeerHash.Random();

                Trigger handler = Trigger.Bind(ref session.Hooks.OnMessageIgnored, data =>
                {
                    data.Peer.Should().NotBeNull();
                    data.Type.Should().Be("found");
                    data.Payload.Should().NotBeNull();
                });

                session.Sender.Send(peer, new SenderFound());
                handler.Wait().Should().BeTrue();
            }
        }
    }
}