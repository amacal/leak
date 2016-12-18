using FluentAssertions;
using Leak.Core.Metaget;
using Leak.Core.Tests.Core;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Core.Tests.Components
{
    public class MetagetTests
    {
        private MetagetHooks hooks;
        private MetagetConfiguration configuration;

        private Fixture fixture;
        private Environment environemnt;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            fixture = new Fixture();
        }

        [OneTimeTearDown]
        public void OneTearDown()
        {
            fixture.Dispose();
            fixture = null;
        }

        [SetUp]
        public void SetUp()
        {
            environemnt = new Environment(fixture.Debian.Metadata);

            configuration = new MetagetConfiguration();
            hooks = new MetagetHooks();
        }

        [TearDown]
        public void TearDown()
        {
            environemnt.Dispose();
            environemnt = null;
        }

        private MetagetService NewMetaGetService()
        {
            return new MetagetService(environemnt.Peers.Bob.Entry.Glue, environemnt.Destination, hooks, configuration);
        }

        [Test]
        public void ShouldTriggerMetafileMeasured()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetafileMeasured, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Size.Should().Be(fixture.Debian.Metadata.Size);
            });

            using (MetagetService metaget = NewMetaGetService())
            {
                metaget.Start(environemnt.Pipeline);
                metaget.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileDiscoveredWhenInitiallyDone()
        {
            environemnt.CompleteMetainfoFile();

            Trigger handler = Trigger.Bind(ref hooks.OnMetadataDiscovered, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(fixture.Debian.Metadata.Hash);
            });

            using (MetagetService metaget = NewMetaGetService())
            {
                metaget.Start(environemnt.Pipeline);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileDiscoveredWhenPopulated()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetadataDiscovered, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(fixture.Debian.Metadata.Hash);
            });

            using (MetagetService metaget = NewMetaGetService())
            {
                metaget.Start(environemnt.Pipeline);

                metaget.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                metaget.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}