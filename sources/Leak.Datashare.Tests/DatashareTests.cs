using FakeItEasy;
using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Peer.Coordinator.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Data.Share.Tests
{
    public class DataShareTests
    {
        [Test]
        public void ShouldReadFromRepositoryWhenBlockRequested()
        {
            using (DataShareFixture fixture = new DataShareFixture())
            using (DataShareSession session = fixture.Start())
            {
                BlockRequested requested = new BlockRequested
                {
                    Hash = session.Service.Hash,
                    Peer = PeerHash.Random(),
                    Block = new BlockIndex(1, 0, 3616)
                };

                session.Service.Start();
                session.Service.Handle(requested);
                session.Pipeline.Process();

                A.CallTo(() => session.DataStore.Read(requested.Block)).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldTriggerBlockSent()
        {
            using (DataShareFixture fixture = new DataShareFixture())
            using (DataShareSession session = fixture.Start())
            {
                BlockRequested requested = new BlockRequested
                {
                    Hash = session.Service.Hash,
                    Peer = PeerHash.Random(),
                    Block = new BlockIndex(1, 0, 3616)
                };

                BlockRead read = new BlockRead
                {
                    Hash = session.Service.Hash,
                    Block = requested.Block,
                    Payload = new DataShareBlock(3616)
                };

                Trigger handler = Trigger.Bind(ref session.Service.Hooks.OnBlockSent, data =>
                {
                    data.Hash.Should().Be(session.Service.Hash);
                    data.Peer.Should().Be(requested.Peer);
                    data.Block.Should().Be(requested.Block);
                    data.Block.Size.Should().Be(3616);
                });

                session.Service.Start();
                session.Service.Handle(requested);

                session.Service.Handle(read);
                session.Pipeline.Process();

                handler.Wait().Should().BeTrue();
            }
        }
    }
}