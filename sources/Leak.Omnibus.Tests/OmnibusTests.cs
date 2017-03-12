using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Datamap.Tests
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

                session.Service.Start();
                session.Service.HandleMetafileVerified(session.Metainfo);
                session.Service.HandleDataVerified(session.Metainfo.Pieces.Length);

                session.Service.Complete(1);
                session.Pipeline.Process();

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

                session.Service.Start();
                session.Service.HandleMetafileVerified(session.Metainfo);
                session.Service.HandleDataVerified(session.Metainfo.Pieces.Length);

                session.Service.Complete(0);
                session.Service.Complete(1);
                session.Pipeline.Process();

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
                    data.Piece.Index.Should().Be(1);
                });

                session.Service.Start();
                session.Service.HandleMetafileVerified(session.Metainfo);
                session.Service.HandleDataVerified(session.Metainfo.Pieces.Length);

                session.Service.Complete(new BlockIndex(1, 0, 16384));
                session.Pipeline.Process();

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

                session.Service.Start();
                session.Service.HandleMetafileVerified(session.Metainfo);
                session.Service.HandleDataVerified(session.Metainfo.Pieces.Length);

                session.Service.Complete(new BlockIndex(1, 0, 16384));
                session.Service.Complete(1);
                session.Pipeline.Process();

                trigger.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerBlockReserved()
        {
            PeerBitfieldChanged bitfieldChanged = new PeerBitfieldChanged
            {
                Peer = PeerHash.Random(),
                Bitfield = Bitfield.Sequence(false, true, false),
            };

            PeerStatusChanged statusChanged = new PeerStatusChanged()
            {
                Peer = bitfieldChanged.Peer,
                State = new PeerState
                {
                    IsLocalInterestedInRemote = true,
                    IsRemoteChokingLocal = false
                }
            };

            using (OmnibusFixture fixture = new OmnibusFixture())
            using (OmnibusSession session = fixture.Start())
            {
                Trigger trigger = Trigger.Bind(ref session.Hooks.OnBlockReserved, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Peer.Should().Be(bitfieldChanged.Peer);
                    data.Block.Should().NotBeNull();
                    data.Block.Piece.Index.Should().Be(1);
                    data.Block.Offset.Should().Be(0);
                    data.Block.Size.Should().Be(3616);
                });

                session.Service.Start();
                session.Service.HandleMetafileVerified(session.Metainfo);
                session.Service.HandleDataVerified(session.Metainfo.Pieces.Length);

                session.Service.Handle(bitfieldChanged);
                session.Service.Handle(statusChanged);

                session.Service.Schedule(OmnibusStrategy.RarestFirst, bitfieldChanged.Peer, 1);
                session.Pipeline.Process();

                trigger.Wait().Should().BeTrue();
            }
        }
    }
}