using F2F.Sandbox;
using FluentAssertions;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Spartan;
using Leak.Core.Tests.Core;
using NUnit.Framework;
using System.IO;

namespace Leak.Core.Tests.Components
{
    public class SpartanTests
    {
        private Swarm swarm;
        private FileHash hash;
        private byte[] metadata;

        private SpartanHooks hooks;
        private SpartanConfiguration configuration;

        private FileSandbox sandbox;
        private string destination;
        private LeakPipeline pipeline;

        private GlueService bob;
        private GlueService sue;

        [SetUp]
        public void SetUp()
        {
            Fixture.Metainfo(out hash, out metadata);

            swarm = new Swarm(hash);
            swarm.Start();

            swarm.Listen("bob", 8091);
            swarm.Connect("sue", 8091);

            swarm["bob"].Exchanged.Wait();
            swarm["sue"].Exchanged.Wait();

            hooks = new SpartanHooks();
            configuration = new SpartanConfiguration();

            sandbox = Sandbox.New();
            destination = Path.Combine(sandbox.Directory, hash.ToString());

            pipeline = new LeakPipeline();
            pipeline.Start();

            bob = swarm["bob"].Glue;
            sue = swarm["bob"].Glue;
        }

        [TearDown]
        public void TearDown()
        {
            swarm.Dispose();
            pipeline.Stop();
            sandbox.Dispose();
        }

        [Test]
        public void ShouldTriggerTaskStartedWithDiscovery()
        {
            var handler = hooks.OnTaskStarted.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Task.Should().Be(SpartanTasks.Discover);
            });

            hooks.OnTaskStarted = handler;
            configuration.Tasks = SpartanTasks.Discover;

            using (SpartanService spartan = new SpartanService(pipeline, destination, bob, hooks, configuration))
            {
                spartan.Start();
            }

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetadataMeasuredWhenReceivedTotalSize()
        {
            MetadataMeasured measured = new MetadataMeasured
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Size = 7186
            };

            var handler = hooks.OnMetadataMeasured.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Size.Should().Be(7186);
            });

            hooks.OnMetadataMeasured = handler;
            configuration.Tasks = SpartanTasks.Discover;

            using (SpartanService spartan = new SpartanService(pipeline, destination, bob, hooks, configuration))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(measured);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataDiscoveredWhenReceivedAllMetadata()
        {
            MetadataMeasured measured = new MetadataMeasured
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Size = metadata.Length
            };

            MetadataReceived received = new MetadataReceived
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Piece = 0,
                Data = metadata
            };

            var handler = hooks.OnMetadataDiscovered.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(hash);
            });

            hooks.OnMetadataDiscovered = handler;
            configuration.Tasks = SpartanTasks.Discover;

            using (SpartanService spartan = new SpartanService(pipeline, destination, bob, hooks, configuration))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(measured);
                spartan.HandleMetadataReceived(received);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerTaskCompletedWhenReceivedAllMetadata()
        {
            MetadataMeasured measured = new MetadataMeasured
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Size = metadata.Length
            };

            MetadataReceived received = new MetadataReceived
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Piece = 0,
                Data = metadata
            };

            var handler = hooks.OnTaskCompleted.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Task.Should().Be(SpartanTasks.Discover);
            });

            hooks.OnTaskCompleted = handler;
            configuration.Tasks = SpartanTasks.Discover;

            using (SpartanService spartan = new SpartanService(pipeline, destination, bob, hooks, configuration))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(measured);
                spartan.HandleMetadataReceived(received);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetadataPieceRequested()
        {
            MetadataMeasured measured = new MetadataMeasured
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Size = metadata.Length
            };

            var handler = hooks.OnMetadataPieceRequested.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Peer.Should().NotBeNull();
                data.Piece.Should().Be(0);
            });

            hooks.OnMetadataPieceRequested = handler;
            configuration.Tasks = SpartanTasks.Discover;

            using (SpartanService spartan = new SpartanService(pipeline, destination, bob, hooks, configuration))
            {
                spartan.Start();
                spartan.HandleMetadataMeasured(measured);

                handler.Wait().Should().BeTrue();
            }
        }
    }
}