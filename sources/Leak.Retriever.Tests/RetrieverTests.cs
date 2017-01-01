using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Retriever.Tests
{
    public class RetrieverTests
    {
        [Test]
        public void ShouldTriggerDataHandled()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnBlockHandled, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Peer.Should().NotBeNull();
                    data.Piece.Should().Be(1);
                    data.Block.Should().Be(0);
                    data.Size.Should().Be(3616);
                });

                session.Service.Start();
                session.Service.HandleBlockReceived(1, session.Data[1]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataRequested()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                PeerChanged changed = new PeerChanged
                {
                    Peer = PeerHash.Random(),
                    Bitfield = Bitfield.Sequence(false, true, false),
                    IsLocalInterestedInRemote = true
                };

                BlockReserved reserved = new BlockReserved
                {
                    Hash = session.Hash,
                    Peer = changed.Peer,
                    Piece = 1,
                    Block = 0,
                    Size = 3616
                };

                Trigger handler = Trigger.Bind(ref session.Hooks.OnBlockRequested, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Peer.Should().Be(changed.Peer);
                    data.Piece.Should().Be(1);
                    data.Block.Should().Be(0);
                });

                session.Service.Start();
                session.Service.Handle(changed);
                session.Service.Handle(reserved);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}