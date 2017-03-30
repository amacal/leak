using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Data.Share.Tests
{
    public class DataShareTests
    {
        [Test]
        public void ShouldTriggerBlockReadOnRepository()
        {
            using (DataShareFixture fixture = new DataShareFixture())
            using (DataShareSession session = fixture.Start())
            {
                MetafileVerified verified = new MetafileVerified
                {
                    Hash = session.Repository.Hash,
                    Metainfo = session.Metainfo
                };

                BlockRequested requested = new BlockRequested
                {
                    Hash = session.Datashare.Hash,
                    Peer = PeerHash.Random(),
                    Block = new BlockIndex(1, 0, 3616)
                };

                Trigger handler = Trigger.Bind(ref session.Repository.Hooks.OnBlockRead, data =>
                {
                    data.Hash.Should().Be(session.Datashare.Hash);
                    data.Block.Should().Be(requested.Block);
                    data.Payload.Size.Should().Be(3616);
                });

                session.Repository.Start();
                session.Repository.Handle(verified);

                session.Datashare.Start();
                session.Datashare.Handle(requested);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerBlockSentOnDatashare()
        {
            using (DataShareFixture fixture = new DataShareFixture())
            using (DataShareSession session = fixture.Start())
            {
                MetafileVerified verified = new MetafileVerified
                {
                    Hash = session.Repository.Hash,
                    Metainfo = session.Metainfo
                };

                BlockRequested requested = new BlockRequested
                {
                    Hash = session.Datashare.Hash,
                    Peer = PeerHash.Random(),
                    Block = new BlockIndex(1, 0, 3616)
                };

                BlockRead read = new BlockRead
                {
                    Hash = session.Repository.Hash,
                    Block = requested.Block,
                    Payload = new DataShareBlock(session.Data[1])
                };

                Trigger handler = Trigger.Bind(ref session.Datashare.Hooks.OnBlockSent, data =>
                {
                    data.Hash.Should().Be(session.Datashare.Hash);
                    data.Peer.Should().Be(requested.Peer);
                    data.Block.Should().Be(requested.Block);
                    data.Block.Size.Should().Be(3616);
                });

                session.Repository.Start();
                session.Repository.Handle(verified);

                session.Datashare.Start();
                session.Datashare.Handle(requested);
                session.Datashare.Handle(read);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}