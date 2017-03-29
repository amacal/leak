using System;
using FakeItEasy;
using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Data.Get.Tests
{
    public class DataGetTests
    {
        [Test]
        public void ShouldTriggerBlockHandled()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                BlockReceived received = new BlockReceived
                {
                    Payload = new DataGetBlock(),
                    Peer = PeerHash.Random(),
                    Hash = session.Service.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                Trigger handler = Trigger.Bind(ref session.Service.Hooks.OnBlockHandled, data =>
                {
                    data.Hash.Should().Be(session.Service.Hash);
                    data.Peer.Should().NotBeNull();
                    data.Block.Should().Be(received.Block);
                    data.Block.Size.Should().Be(3616);
                });

                session.Service.Start();
                session.Service.Handle(received);

                session.Pipeline.Process();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldWriteToRepositoryWhenBlockReceived()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                BlockReceived received = new BlockReceived
                {
                    Payload = new DataGetBlock(),
                    Peer = PeerHash.Random(),
                    Hash = session.Service.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Service.Start();
                session.Service.Handle(received);
                session.Pipeline.Process();

                A.CallTo(() => session.DataStore.Write(received.Block, received.Payload)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldTriggerBlockRequested()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                BlockReserved reserved = new BlockReserved
                {
                    Hash = session.Service.Hash,
                    Peer = PeerHash.Random(),
                    Block = new BlockIndex(1, 0, 3616)
                };

                Trigger handler = Trigger.Bind(ref session.Service.Hooks.OnBlockRequested, data =>
                {
                    data.Hash.Should().Be(session.Service.Hash);
                    data.Peer.Should().Be(reserved.Peer);
                    data.Block.Should().Be(reserved.Block);
                    data.Block.Size.Should().Be(3616);
                });

                session.Service.Start();
                session.Service.Handle(reserved);

                session.Pipeline.Process();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldSendRequestWhenBlockReserved()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                BlockReserved received = new BlockReserved
                {
                    Peer = PeerHash.Random(),
                    Hash = session.Service.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Service.Start();
                session.Service.Handle(received);
                session.Pipeline.Process();

                A.CallTo(() => session.Glue.SendRequest(received.Peer, received.Block)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldVerifyPieceWhenPieceIsReady()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                PieceReady ready = new PieceReady
                {
                    Hash = session.Service.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Service.Start();
                session.Service.Handle(ready);
                session.Pipeline.Process();

                A.CallTo(() => session.DataStore.Verify(ready.Piece)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldCompletePieceWhenPieceIsAccepted()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                PieceAccepted accepted = new PieceAccepted
                {
                    Hash = session.Service.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Service.Start();
                session.Service.Handle(accepted);
                session.Pipeline.Process();

                A.CallTo(() => session.DataMap.Complete(accepted.Piece)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldInvalidatePieceWhenPieceIsRejected()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                PieceRejected rejected = new PieceRejected
                {
                    Hash = session.Service.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Service.Start();
                session.Service.Handle(rejected);
                session.Pipeline.Process();

                A.CallTo(() => session.DataMap.Invalidate(rejected.Piece)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldCompleteBlockWhenBlockIsWritten()
        {
            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                BlockWritten written = new BlockWritten
                {
                    Hash = session.Service.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Service.Start();
                session.Service.Handle(written);
                session.Pipeline.Process();

                A.CallTo(() => session.DataMap.Complete(written.Block)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldSentInterestedWhenPeerHasSomethingToOffer()
        {
            PeerHash other = PeerHash.Random();

            Action<Action<PeerHash, Bitfield, PeerState>> handle = callback =>
            {
                callback.Invoke(other, Bitfield.Complete(1), new PeerState());
            };

            using (DataGetFixture fixture = new DataGetFixture())
            using (DataGetSession session = fixture.Start())
            {
                DataVerified verified = new DataVerified
                {
                    Hash = session.Service.Parameters.Hash,
                    Bitfield = new Bitfield(1)
                };

                A.CallTo(() => session.DataMap.Query(A<Action<PeerHash, Bitfield, PeerState>>.Ignored)).Invokes(handle);

                session.Service.Start();
                session.Service.Handle(verified);

                session.Pipeline.Tick();
                session.Pipeline.Process();

                A.CallTo(() => session.Glue.SendInterested(other)).MustHaveHappened();
            }
        }
    }
}