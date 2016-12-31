using FluentAssertions;
using Leak.Common;
using Leak.Extensions.Metadata;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Metashare.Tests
{
    public class MetashareTests
    {
        [Test]
        public void ShouldTriggerMetadataShared()
        {
            using (MetashareFixture fixture = new MetashareFixture())
            using (MetashareSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetadataShared, data =>
                {
                    data.Hash.Should().Be(session.Service.Hash);
                    data.Peer.Should().NotBeNull();
                    data.Piece.Should().Be(0);
                });

                session.Service.Start();
                session.Service.Handle(new MetadataRequested
                {
                    Hash = session.Service.Hash,
                    Peer = PeerHash.Random(),
                    Piece = 0
                });

                handler.Wait().Should().BeTrue();
            }
        }
    }
}
