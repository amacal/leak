using FluentAssertions;
using Leak.Core.Common;
using Leak.Core.Tests.Stubs;
using NUnit.Framework;
using System;

namespace Leak.Core.Tests.Negotiator
{
    [TestFixture]
    public class NegotiatorTests
    {
        [Test]
        public void CanEstablishConnectionForGivenHash()
        {
            FileHash hash = new FileHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                HandshakeNegotiatorActiveStub active = new HandshakeNegotiatorActiveStub(container.Pool, container["active"], with => with.Hash = hash);
                HandshakeNegotiatorPassiveStub passive = new HandshakeNegotiatorPassiveStub(container.Pool, container["passive"], with => with.Hashes.Add(hash));

                passive.Execute();
                active.Execute();

                active.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));
                passive.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));

                active.Context.Handshake.Hash.ToBytes().Should().Equal(hash.ToBytes());
                passive.Context.Handshake.Hash.ToBytes().Should().Equal(hash.ToBytes());
            }
        }

        [Test]
        public void CanEstablishConnectionForGivenPeers()
        {
            FileHash hash = new FileHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                HandshakeNegotiatorActiveStub active = new HandshakeNegotiatorActiveStub(container.Pool, container["active"], with => with.Hash = hash);
                HandshakeNegotiatorPassiveStub passive = new HandshakeNegotiatorPassiveStub(container.Pool, container["passive"], with => with.Hashes.Add(hash));

                passive.Execute();
                active.Execute();

                active.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));
                passive.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));

                active.Context.Handshake.Remote.ToBytes().Should().Equal(passive.Context.Peer.ToBytes());
                passive.Context.Handshake.Remote.ToBytes().Should().Equal(active.Context.Peer.ToBytes());
            }
        }

        [Test]
        public void CanRejectConnectionWithUnknownHash()
        {
            FileHash hash = new FileHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                HandshakeNegotiatorActiveStub active = new HandshakeNegotiatorActiveStub(container.Pool, container["active"], with => with.Hash = hash);
                HandshakeNegotiatorPassiveStub passive = new HandshakeNegotiatorPassiveStub(container.Pool, container["passive"], with => { });

                passive.Execute();
                active.Execute();

                active.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));
                passive.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));

                passive.Context.Rejection.Match.Matches(hash).Should().BeTrue();
                active.Context.IsClosed.Should().BeTrue();
            }
        }

        [Test]
        public void CanAcceptConnectionsWithKnownHash()
        {
            FileHash hash1 = new FileHash(Bytes.Random(20));
            FileHash hash2 = new FileHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                HandshakeNegotiatorActiveStub active = new HandshakeNegotiatorActiveStub(container.Pool, container["active"], with => with.Hash = hash2);
                HandshakeNegotiatorPassiveStub passive = new HandshakeNegotiatorPassiveStub(container.Pool, container["passive"], with => with.Hashes.Add(hash1));

                passive.Execute();
                active.Execute();

                active.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));
                passive.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));

                active.Context.Handshake.Hash.ToBytes().Should().Equal(hash2.ToBytes());
                passive.Context.Handshake.Hash.ToBytes().Should().Equal(hash2.ToBytes());
            }
        }
    }
}