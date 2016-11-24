using FluentAssertions;
using Leak.Core.Spartan;
using Leak.Core.Tests.Core;
using NUnit.Framework;

namespace Leak.Core.Tests.Components
{
    public class SpartanTests
    {
        private SpartanHooks hooks;
        private SpartanConfiguration configuration;

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
            environemnt = new Environment(fixture.Metadata.Debian);

            configuration = new SpartanConfiguration();
            hooks = new SpartanHooks();
        }

        [TearDown]
        public void TearDown()
        {
            environemnt.Dispose();
            environemnt = null;
        }

        private SpartanService NewSpartanService(SpartanTasks task)
        {
            configuration.Tasks = task;
            return new SpartanService(environemnt.Pipeline, environemnt.Destination, environemnt.Peers.Bob.Entry.Glue, hooks, configuration);
        }

        [Test]
        public void ShouldTriggerTaskStartedWithDiscovery()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnTaskStarted, data =>
            {
                data.Hash.Should().Be(fixture.Metadata.Debian.Hash);
                data.Task.Should().Be(SpartanTasks.Discover);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileMeasuredWhenReceivedTotalSize()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetafileMeasured, data =>
            {
                data.Hash.Should().Be(fixture.Metadata.Debian.Hash);
                data.Size.Should().Be(fixture.Metadata.Debian.Size);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(fixture.Metadata.Debian.Events.MetadataMeasured);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataDiscoveredWhenReceivedAllMetadata()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetadataDiscovered, data =>
            {
                data.Hash.Should().Be(fixture.Metadata.Debian.Hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(data.Hash);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(fixture.Metadata.Debian.Events.MetadataMeasured);
                spartan.HandleMetadataReceived(fixture.Metadata.Debian.Events.MetadataReceived);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenReceivedAllMetadata()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnTaskCompleted, data =>
            {
                data.Hash.Should().Be(fixture.Metadata.Debian.Hash);
                data.Task.Should().Be(SpartanTasks.Discover);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(fixture.Metadata.Debian.Events.MetadataMeasured);
                spartan.HandleMetadataReceived(fixture.Metadata.Debian.Events.MetadataReceived);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataPieceRequested()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetadataPieceRequested, data =>
            {
                data.Hash.Should().Be(fixture.Metadata.Debian.Hash);
                data.Peer.Should().NotBeNull();
                data.Piece.Should().Be(0);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(fixture.Metadata.Debian.Events.MetadataMeasured);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}