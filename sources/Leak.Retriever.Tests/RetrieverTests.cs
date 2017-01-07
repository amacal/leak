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
        public void ShouldTriggerBlockHandled()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                BlockReceived received = new BlockReceived
                {
                    Payload = new RetrieverBlock(),
                    Peer = PeerHash.Random(),
                    Hash = session.Retriever.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                Trigger handler = Trigger.Bind(ref session.Retriever.Hooks.OnBlockHandled, data =>
                {
                    data.Hash.Should().Be(session.Retriever.Hash);
                    data.Peer.Should().NotBeNull();
                    data.Block.Should().Be(received.Block);
                    data.Block.Size.Should().Be(3616);
                });

                session.Retriever.Start();
                session.Retriever.Handle(received);

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
                    Hash = session.Retriever.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Retriever.Start();
                session.Retriever.Handle(received);

                session.Pipeline.Process();
                session.Repository.Verify(x => x.Write(received.Block, received.Payload));
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
                    Hash = session.Retriever.Hash,
                    Peer = PeerHash.Random(),
                    Block = new BlockIndex(1, 0, 3616)
                };

                Trigger handler = Trigger.Bind(ref session.Retriever.Hooks.OnBlockRequested, data =>
                {
                    data.Hash.Should().Be(session.Retriever.Hash);
                    data.Peer.Should().Be(reserved.Peer);
                    data.Block.Should().Be(reserved.Block);
                    data.Block.Size.Should().Be(3616);
                });

                session.Retriever.Start();
                session.Retriever.Handle(reserved);

                session.Pipeline.Process();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldSendRequestedWhenBlockReserved()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                BlockReserved received = new BlockReserved
                {
                    Peer = PeerHash.Random(),
                    Hash = session.Retriever.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Retriever.Start();
                session.Retriever.Handle(received);

                session.Pipeline.Process();
                session.Glue.Verify(x => x.SendRequest(received.Peer, received.Block));
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
                    Hash = session.Retriever.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Retriever.Start();
                session.Retriever.Handle(ready);

                session.Pipeline.Process();
                session.Repository.Verify(x => x.Verify(ready.Piece));
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
                    Hash = session.Retriever.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Retriever.Start();
                session.Retriever.Handle(accepted);

                session.Pipeline.Process();
                session.Omnibus.Verify(x => x.Complete(accepted.Piece));
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
                    Hash = session.Retriever.Hash,
                    Piece = new PieceInfo(1)
                };

                session.Retriever.Start();
                session.Retriever.Handle(rejected);

                session.Pipeline.Process();
                session.Omnibus.Verify(x => x.Invalidate(rejected.Piece));
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
                    Hash = session.Retriever.Hash,
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Retriever.Start();
                session.Retriever.Handle(written);

                session.Pipeline.Process();
                session.Omnibus.Verify(x => x.Complete(written.Block));
            }
        }
    }
}