using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Omnibus.Tests
{
    public class OmnibusTests
    {
        [Test]
        public void ShouldTriggerDataChangedWhenPieceCompleted()
        {
            using (OmnibusFixture fixture = new OmnibusFixture())
            using (OmnibusSession session = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref session.Hooks.OnDataChanged, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Completed.Should().Be(1);
                });

                session.Service.Complete(1);
                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataCompletedWhenAllPiecesCompleted()
        {
            using (OmnibusFixture fixture = new OmnibusFixture())
            using (OmnibusSession session = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref session.Hooks.OnDataCompleted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                });

                session.Service.Complete(0);
                session.Service.Complete(1);

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceReadyWhenAllBlocksCompleted()
        {
            using (OmnibusFixture fixture = new OmnibusFixture())
            using (OmnibusSession session = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref session.Hooks.OnPieceReady, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(1);
                });

                session.Service.Complete(new OmnibusBlock(1, 0, 16384));
                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceCompletedWhenAllBlocksCompleted()
        {
            using (OmnibusFixture fixture = new OmnibusFixture())
            using (OmnibusSession session = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref session.Hooks.OnPieceCompleted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(1);
                });

                session.Service.Complete(new OmnibusBlock(1, 0, 16384));
                session.Service.Complete(1);

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerBlockReserved()
        {
            PeerChanged changed = new PeerChanged
            {
                Peer = PeerHash.Random(),
                Bitfield = Bitfield.Sequence(false, true, false),
                IsLocalInterestedInRemote = true,
                IsRemoteChokingLocal = false
            };

            using (OmnibusFixture fixture = new OmnibusFixture())
            using (OmnibusSession session = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref session.Hooks.OnBlockReserved, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Peer.Should().Be(changed.Peer);
                    data.Piece.Should().Be(1);
                    data.Block.Should().Be(0);
                });

                session.Service.Handle(changed);
                session.Service.Schedule(OmnibusStrategy.RarestFirst, changed.Peer, 1);

                trigger.Wait().Should().BeTrue();
            }
        }
    }
}