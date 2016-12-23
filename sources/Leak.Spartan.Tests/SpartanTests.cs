using System.IO;
using FluentAssertions;
using Leak.Common;
using Leak.Events;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Spartan.Tests
{
    public class SpartanTests
    {
        [Test]
        public void ShouldTriggerTaskStartedWithDiscovery()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnTaskStarted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Task.Should().Be(SpartanTasks.Discover);
                });

                session.Service.Start();
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileMeasuredWhenReceivedTotalSize()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileMeasured, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Size.Should().Be(session.Meta.Size);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataDiscoveredWhenReceivedAllMetadata()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetadataDiscovered, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Metainfo.Should().NotBeNull();
                    data.Metainfo.Hash.Should().Be(data.Hash);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenReceivedAllMetadata()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnTaskCompleted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Task.Should().Be(SpartanTasks.Discover);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataAllocated()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover | SpartanTasks.Verify))
            {
                string directory = Path.Combine(session.Sandbox.Directory, session.Hash.ToString());
                string path = Path.Combine(directory, "debian-8.5.0-amd64-CD-1.iso");

                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataAllocated, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Directory.Should().EndWith(session.Directory);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                handler.Wait().Should().BeTrue();
                session.Sandbox.ExistsFile(path).Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataVerified()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover | SpartanTasks.Verify))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataVerified, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Bitfield.Should().NotBeNull();
                    data.Bitfield.Completed.Should().Be(0);
                    data.Bitfield.Length.Should().Be(session.Data.Count);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenVerified()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover | SpartanTasks.Verify))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnTaskCompleted, data =>
                {
                    if (data.Task == SpartanTasks.Verify)
                    {
                        data.Hash.Should().Be(session.Hash);
                    }
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataCompleted()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover | SpartanTasks.Verify | SpartanTasks.Download))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataCompleted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                session.Stage.Downloading.Wait(5000).Should().BeTrue();

                session.Service.HandleBlockReceived(session.Hash, 0, session.Data[0]);
                session.Service.HandleBlockReceived(session.Hash, 1, session.Data[1]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataChanged()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover | SpartanTasks.Verify | SpartanTasks.Download))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataChanged, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Completed.Should().Be(1);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                session.Stage.Downloading.Wait(5000).Should().BeTrue();
                session.Service.HandleBlockReceived(session.Hash, 0, session.Data[0]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceAccepted()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover | SpartanTasks.Verify | SpartanTasks.Download))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnPieceAccepted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(1);
                });

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                session.Stage.Downloading.Wait(5000).Should().BeTrue();
                session.Service.HandleBlockReceived(session.Hash, 1, session.Data[1]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerPieceRejected()
        {
            using (SpartanFixture fixture = new SpartanFixture())
            using (SpartanSession session = fixture.Start(SpartanTasks.Discover | SpartanTasks.Verify | SpartanTasks.Download))
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnPieceRejected, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(1);
                });

                BlockReceived received = new BlockReceived
                {
                    Peer = PeerHash.Random(),
                    Hash = session.Hash,
                    Piece = 1,
                    Block = 0,
                    Payload = new FixedDataBlock(session.Data[0])
                };

                session.Service.Start();
                session.Stage.Discovering.Wait(5000).Should().BeTrue();

                session.Service.HandleMetadataMeasured(session.Hash, session.Meta.Size);
                session.Service.HandleMetadataReceived(session.Hash, 0, session.Meta[0]);

                session.Stage.Downloading.Wait(5000).Should().BeTrue();
                session.Service.HandleBlockReceived(received);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}