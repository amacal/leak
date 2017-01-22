using FakeItEasy;
using FluentAssertions;
using Leak.Common;
using Leak.Extensions.Metadata;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Metaget.Tests
{
    public class MetagetTests
    {
        [Test]
        public void ShouldTriggerMetafileMeasured()
        {
            using (MetagetFixture fixture = new MetagetFixture())
            using (MetagetSession session = fixture.Start())
            {
                MetadataMeasured measured = new MetadataMeasured
                {
                    Hash = session.Hash,
                    Size = session.Size,
                    Peer = PeerHash.Random()
                };

                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileMeasured, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Size.Should().Be(session.Size);
                });

                session.Service.Start();
                session.Service.Handle(measured);

                session.Pipeline.Process();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldVerifyMetafileWhenStarted()
        {
            using (MetagetFixture fixture = new MetagetFixture())
            using (MetagetSession session = fixture.Start(true))
            {
                A.CallTo(() => session.Metafile.IsCompleted()).Returns(false);

                session.Service.Start();
                session.Pipeline.Process();

                A.CallTo(() => session.Metafile.Verify()).MustHaveHappened();
            }
        }

        [Test]
        public void ShouldSendMetadataRequested()
        {
            PeerHash other = PeerHash.Random();
            PeerHash[] peers = { other };

            using (MetagetFixture fixture = new MetagetFixture())
            using (MetagetSession session = fixture.Start())
            {
                MetadataMeasured measured = new MetadataMeasured
                {
                    Hash = session.Hash,
                    Size = session.Size,
                    Peer = PeerHash.Random()
                };

                A.CallTo(() => session.Glue.Peers).Returns(peers);

                session.Service.Start();

                session.Service.Handle(measured);
                session.Pipeline.Process();

                session.Pipeline.Tick();
                session.Pipeline.Process();

                A.CallTo(() => session.Glue.SendMetadataRequest(other, 0)).MustHaveHappened();
            }
        }
    }
}