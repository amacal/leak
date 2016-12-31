using System.IO;
using FluentAssertions;
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
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileMeasured, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Size.Should().Be(session.Size);
                });

                session.Service.Start();
                session.Service.HandleMetadataMeasured(session.Hash, session.Size);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileDiscoveredWhenInitiallyDone()
        {
            using (MetagetFixture fixture = new MetagetFixture())
            using (MetagetSession session = fixture.Start(true))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetadataDiscovered, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Metainfo.Should().NotBeNull();
                    data.Metainfo.Hash.Should().Be(session.Hash);
                });

                session.Service.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileDiscoveredWhenPopulated()
        {
            using (MetagetFixture fixture = new MetagetFixture())
            using (MetagetSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetadataDiscovered, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Metainfo.Should().NotBeNull();
                    data.Metainfo.Hash.Should().Be(session.Hash);
                });

                session.Service.Start();

                session.Service.HandleMetadataMeasured(session.Hash, session.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Data[0]);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}