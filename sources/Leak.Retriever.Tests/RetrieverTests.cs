using System;
using FakeItEasy;
using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Dataget.Tests
{
    public class RetrieverTests
    {
        [Test]
        public void ShouldTriggerBlockHandled()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                BlockReceived received = new BlockReceived
                {
                    Payload = new RetrieverBlock(),
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
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                BlockReceived received = new BlockReceived
                {
                    Payload = new RetrieverBlock(),
                    Peer = PeerHash.Random(),
                    Hash = session.Service.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Service.Start();
                session.Service.Handle(received);
                session.Pipeline.Process();

                A.CallTo(() => session.Repository.Write(received.Block, received.Payload)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldTriggerBlockRequested()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
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
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
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
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                PieceReady ready = new PieceReady
                {
                    Hash = session.Service.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Service.Start();
                session.Service.Handle(ready);
                session.Pipeline.Process();

                A.CallTo(() => session.Repository.Verify(ready.Piece)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldCompletePieceWhenPieceIsAccepted()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                PieceAccepted accepted = new PieceAccepted
                {
                    Hash = session.Service.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Service.Start();
                session.Service.Handle(accepted);
                session.Pipeline.Process();

                A.CallTo(() => session.Omnibus.Complete(accepted.Piece)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldInvalidatePieceWhenPieceIsRejected()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                PieceRejected rejected = new PieceRejected
                {
                    Hash = session.Service.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Service.Start();
                session.Service.Handle(rejected);
                session.Pipeline.Process();

                A.CallTo(() => session.Omnibus.Invalidate(rejected.Piece)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldCompleteBlockWhenBlockIsWritten()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                BlockWritten written = new BlockWritten
                {
                    Hash = session.Service.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Service.Start();
                session.Service.Handle(written);
                session.Pipeline.Process();

                A.CallTo(() => session.Omnibus.Complete(written.Block)).MustHaveHappened();
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

            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                A.CallTo(() => session.Omnibus.Query(A<Action<PeerHash, Bitfield, PeerState>>.Ignored)).Invokes(handle);

                session.Service.Start();
                session.Pipeline.Tick();
                session.Pipeline.Process();

                A.CallTo(() => session.Glue.SendInterested(other)).MustHaveHappened();
            }
        }
    }
}