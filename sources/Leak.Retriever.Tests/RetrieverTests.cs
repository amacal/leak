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
        public void ShouldTriggerDataChanged()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataChanged, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Completed.Should().Be(1);
                });

                session.Service.Start();
                session.Service.HandleBlockReceived(session.Hash, 0, session.Data[0]);

                handler.Wait().Should().BeTrue();
            }
        }

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
                session.Service.HandleBlockReceived(session.Hash, 1, session.Data[1]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceAccepted()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnPieceAccepted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(1);
                });

                session.Service.Start();
                session.Service.HandleBlockReceived(session.Hash, 1, session.Data[1]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceRejected()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnPieceRejected, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(1);
                });

                session.Service.Start();
                session.Service.HandleBlockReceived(session.Hash, 1, session.Data[0]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataCompleted()
        {
            using (RetrieverFixture fixture = new RetrieverFixture())
            using (RetrieverSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataCompleted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                });

                session.Service.Start();
                session.Service.HandleBlockReceived(session.Hash, 0, session.Data[0]);
                session.Service.HandleBlockReceived(session.Hash, 1, session.Data[1]);

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

                Trigger handler = Trigger.Bind(ref session.Hooks.OnBlockRequested, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Peer.Should().Be(changed.Peer);
                    data.Piece.Should().Be(1);
                    data.Block.Should().Be(0);
                });

                session.Service.Start();
                session.Service.HandlePeerChanged(changed);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}