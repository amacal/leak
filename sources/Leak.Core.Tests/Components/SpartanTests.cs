using FluentAssertions;
using Leak.Core.Events;
using Leak.Core.Spartan;
using Leak.Core.Tests.Core;
using NUnit.Framework;
using System.IO;

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
            environemnt = new Environment(fixture.Debian.Metadata);

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

            return new SpartanService(environemnt.Pipeline, environemnt.Destination, environemnt.Peers.Bob.Entry.Glue, environemnt.Files, hooks, configuration);
        }

        [Test]
        public void ShouldTriggerTaskStartedWithDiscovery()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnTaskStarted, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
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
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Size.Should().Be(fixture.Debian.Metadata.Size);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataDiscoveredWhenReceivedAllMetadata()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetadataDiscovered, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(data.Hash);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataPieceReceivedWhenReceivedAllMetadata()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetadataPieceReceived, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Peer.Should().NotBeNull();
                data.Piece.Should().Be(0);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenReceivedAllMetadata()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnTaskCompleted, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Task.Should().Be(SpartanTasks.Discover);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataPieceRequested()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnMetadataPieceRequested, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Peer.Should().NotBeNull();
                data.Piece.Should().Be(0);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataAllocated()
        {
            string path = Path.Combine(environemnt.Sandbox.Directory, fixture.Debian.Metadata.Hash.ToString());
            string file = Path.Combine(path, "debian-8.5.0-amd64-CD-1.iso");

            Trigger handler = Trigger.Bind(ref hooks.OnDataAllocated, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Directory.Should().EndWith(path);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover | SpartanTasks.Verify))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }

            environemnt.Sandbox.ExistsFile(file).Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerDataVerified()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnDataVerified, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Bitfield.Should().NotBeNull();
                data.Bitfield.Completed.Should().Be(0);
                data.Bitfield.Length.Should().Be(fixture.Debian.Binary.Pieces.Length);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover | SpartanTasks.Verify))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenVerified()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnTaskCompleted, data =>
            {
                if (data.Task == SpartanTasks.Verify)
                {
                    data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                }
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover | SpartanTasks.Verify))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataCompleted()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnDataCompleted, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover | SpartanTasks.Verify | SpartanTasks.Download))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }

                    if (data.Task == SpartanTasks.Download)
                    {
                        foreach (BlockReceived item in fixture.Debian.Events.BlockReceived)
                        {
                            spartan.HandleBlockReceived(item);
                        }
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataChanged()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnDataChanged, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Completed.Should().Be(1);
            });

            using (SpartanService spartan = NewSpartanService(SpartanTasks.Discover | SpartanTasks.Verify | SpartanTasks.Download))
            {
                hooks.OnTaskStarted = data =>
                {
                    if (data.Task == SpartanTasks.Discover)
                    {
                        spartan.HandleMetadataMeasured(fixture.Debian.Events.MetadataMeasured);
                        spartan.HandleMetadataReceived(fixture.Debian.Events.MetadataReceived);
                    }

                    if (data.Task == SpartanTasks.Download)
                    {
                        spartan.HandleBlockReceived(fixture.Debian.Events.BlockReceived[1]);
                    }
                };

                spartan.Start();
                handler.Wait().Should().BeTrue();
            }
        }
    }
}