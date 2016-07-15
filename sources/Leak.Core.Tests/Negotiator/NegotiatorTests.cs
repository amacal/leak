using FluentAssertions;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Leak.Core.Tests.Negotiator
{
    [TestFixture]
    public class NegotiatorTests
    {
        [Test]
        public void CanEstablishConnectionForGivenHash()
        {
            HandshakeHash hash = new HandshakeHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                ActiveNegotiator active = new ActiveNegotiator(container["active"], with => with.Hash = hash);
                PassiveNegotiator passive = new PassiveNegotiator(container["passive"], with => with.Hashes.Add(hash));

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
            HandshakeHash hash = new HandshakeHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                ActiveNegotiator active = new ActiveNegotiator(container["active"], with => with.Hash = hash);
                PassiveNegotiator passive = new PassiveNegotiator(container["passive"], with => with.Hashes.Add(hash));

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
            HandshakeHash hash = new HandshakeHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                ActiveNegotiator active = new ActiveNegotiator(container["active"], with => with.Hash = hash);
                PassiveNegotiator passive = new PassiveNegotiator(container["passive"], with => { });

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
            HandshakeHash hash1 = new HandshakeHash(Bytes.Random(20));
            HandshakeHash hash2 = new HandshakeHash(Bytes.Random(20));

            using (NetworkContainer container = new NetworkContainer())
            {
                container.Active("active");
                container.Passive("passive");
                container.Connect("active", "passive");

                ActiveNegotiator active = new ActiveNegotiator(container["active"], with => with.Hash = hash2);
                PassiveNegotiator passive = new PassiveNegotiator(container["passive"], with => with.Hashes.Add(hash1));

                passive.Execute();
                active.Execute();

                active.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));
                passive.Context.Ready.WaitOne(TimeSpan.FromSeconds(2));

                active.Context.Handshake.Hash.ToBytes().Should().Equal(hash2.ToBytes());
                passive.Context.Handshake.Hash.ToBytes().Should().Equal(hash2.ToBytes());
            }
        }

        private class ActiveConfiguration
        {
            public HandshakeHash Hash { get; set; }

            public HandshakePeer Peer { get; set; }
        }

        private class ActiveNegotiator
        {
            private readonly ActiveContext context;
            private readonly HandshakeNegotiatorActive negotiator;

            public ActiveNegotiator(NetworkConnection connection, Action<ActiveConfiguration> configurer)
            {
                context = new ActiveContext(configurer);
                negotiator = new HandshakeNegotiatorActive(connection, context);
            }

            public ActiveContext Context
            {
                get { return context; }
            }

            public void Execute()
            {
                negotiator.Execute();
            }
        }

        private class ActiveContext : HandshakeNegotiatorActiveContext
        {
            private readonly ActiveConfiguration configuration;
            private readonly ManualResetEvent onReady;

            private Handshake foundHandshake;
            private bool isClosed;

            public ActiveContext(Action<ActiveConfiguration> configurer)
            {
                this.configuration = new ActiveConfiguration
                {
                    Hash = new HandshakeHash(Bytes.Random(20)),
                    Peer = new HandshakePeer(Bytes.Random(20))
                };

                configurer.Invoke(this.configuration);
                this.onReady = new ManualResetEvent(false);
            }

            public Handshake Handshake
            {
                get { return foundHandshake; }
            }

            public bool IsClosed
            {
                get { return isClosed; }
            }

            public WaitHandle Ready
            {
                get { return onReady; }
            }

            public HandshakePeer Peer
            {
                get { return configuration.Peer; }
            }

            public HandshakeHash Hash
            {
                get { return configuration.Hash; }
            }

            public HandshakeOptions Options
            {
                get { return HandshakeOptions.None; }
            }

            public void OnHandshake(NetworkConnection connection, Handshake handshake)
            {
                foundHandshake = handshake;
                onReady.Set();
            }

            public void OnException(Exception ex)
            {
                isClosed = true;
                onReady.Set();
            }

            public void OnDisconnected()
            {
                isClosed = true;
                onReady.Set();
            }
        }

        private class PassiveConfiguration
        {
            public List<HandshakeHash> Hashes { get; set; }

            public HandshakePeer Peer { get; set; }
        }

        private class PassiveNegotiator
        {
            private readonly PassiveContext context;
            private readonly HandshakeNegotiatorPassive negotiator;

            public PassiveNegotiator(NetworkConnection connection, Action<PassiveConfiguration> configurer)
            {
                context = new PassiveContext(configurer);
                negotiator = new HandshakeNegotiatorPassive(connection, context);
            }

            public PassiveContext Context
            {
                get { return context; }
            }

            public void Execute()
            {
                negotiator.Execute();
            }
        }

        private class PassiveContext : HandshakeNegotiatorPassiveContext
        {
            private readonly PassiveConfiguration configuration;
            private readonly ManualResetEvent onReady;

            private Handshake foundHandshake;
            private HandshakeRejection foundRejection;

            public PassiveContext(Action<PassiveConfiguration> configurer)
            {
                this.configuration = new PassiveConfiguration
                {
                    Hashes = new List<HandshakeHash>(),
                    Peer = new HandshakePeer(Bytes.Random(20))
                };

                configurer.Invoke(this.configuration);
                this.onReady = new ManualResetEvent(false);
            }

            public Handshake Handshake
            {
                get { return foundHandshake; }
            }

            public HandshakeRejection Rejection
            {
                get { return foundRejection; }
            }

            public WaitHandle Ready
            {
                get { return onReady; }
            }

            public HandshakePeer Peer
            {
                get { return configuration.Peer; }
            }

            public HandshakeOptions Options
            {
                get { return HandshakeOptions.None; }
            }

            public HandshakeHashCollection Hashes
            {
                get { return new HandshakeHashCollection(configuration.Hashes.ToArray()); }
            }

            public void OnRejected(HandshakeRejection rejection)
            {
                foundRejection = rejection;
                onReady.Set();
            }

            public void OnHandshake(NetworkConnection connection, Handshake handshake)
            {
                foundHandshake = handshake;
                onReady.Set();
            }

            public void OnException(Exception ex)
            {
            }

            public void OnDisconnected()
            {
            }
        }
    }
}