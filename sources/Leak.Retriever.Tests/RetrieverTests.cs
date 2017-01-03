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
        public void ShouldTriggerDataHandledOnRetriever()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Retriever.Hooks.OnBlockHandled, data =>
                {
                    data.Hash.Should().Be(session.Retriever.Hash);
                    data.Peer.Should().NotBeNull();
                    data.Block.Piece.Should().Be(1);
                    data.Block.Offset.Should().Be(0);
                    data.Block.Size.Should().Be(3616);
                });

                session.Retriever.Start();
                session.Retriever.HandleBlockReceived(1, session.Data[1]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldCallWriteOnRepository()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                BlockReceived received = new BlockReceived
                {
                    Payload = new RetrieverBlock(session.Data[1]),
                    Peer = PeerHash.Random(),
                    Hash = session.Retriever.Hash,
                    Block = new BlockIndex(1, 0, session.Data[1].Length)
                };

                session.Retriever.Start();
                session.Retriever.Handle(received);

                session.Repository.Verify(x => x.Write(received.Block, received.Payload));
            }
        }

        [Test]
        public void ShouldTriggerDataRequestedOnRetriever()
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
                    Hash = session.Retriever.Hash,
                    Peer = changed.Peer,
                    Block = new BlockIndex(1, 0, 3616)
                };

                Trigger handler = Trigger.Bind(ref session.Retriever.Hooks.OnBlockRequested, data =>
                {
                    data.Hash.Should().Be(session.Retriever.Hash);
                    data.Peer.Should().Be(changed.Peer);
                    data.Block.Piece.Should().Be(1);
                    data.Block.Offset.Should().Be(0);
                    data.Block.Size.Should().Be(3616);
                });

                session.Retriever.Start();
                session.Retriever.Handle(reserved);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}