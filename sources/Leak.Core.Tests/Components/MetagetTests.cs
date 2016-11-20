using F2F.Sandbox;
using FluentAssertions;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Metaget;
using Leak.Core.Tests.Core;
using NUnit.Framework;
using System.IO;

namespace Leak.Core.Tests.Components
{
    public class MetagetTests
    {
        private LeakPipeline pipeline;
        private CompletionThread worker;
        private FileSandbox sandbox;

        private FileHash hash;
        private MetagetService metaget;
        private MetagetHooks hooks;
        private MetagetConfiguration configuration;

        private Swarm swarm;

        private byte[] metadata;
        private string path;

        [SetUp]
        public void SetUp()
        {
            Fixture.Metainfo(out hash, out metadata);

            pipeline = new LeakPipeline();
            worker = new CompletionThread();

            sandbox = Sandbox.New();
            path = sandbox.CreateFile($"{hash}.metainfo");

            worker.Start();
            pipeline.Start();

            swarm = new Swarm(hash);
            swarm.Start();

            swarm.Listen("bob", 8091);
            swarm.Connect("sue", 8091);

            swarm["bob"].Exchanged.Wait();
            swarm["sue"].Exchanged.Wait();

            hooks = new MetagetHooks();
            configuration = new MetagetConfiguration();
            metaget = new MetagetService(swarm["bob"].Glue, sandbox.CreateDirectory($"{hash}"), hooks, configuration);
        }

        [TearDown]
        public void TearDown()
        {
            worker.Dispose();
            pipeline.Stop();
            sandbox.Dispose();
            metaget.Stop(pipeline);
            swarm.Dispose();
        }

        [Test]
        public void ShouldTriggerMetafileMeasured()
        {
            MetadataMeasured measured = new MetadataMeasured
            {
                Hash = hash,
                Peer = PeerHash.Random(),
                Size = 17456
            };

            var handler = hooks.OnMetadataMeasured.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Size.Should().Be(17456);
            });

            metaget.Start(pipeline);
            hooks.OnMetadataMeasured = handler;

            metaget.HandleMetadataMeasured(measured);
            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetafileDiscoveredWhenInitiallyDone()
        {
            var handler = hooks.OnMetadataDiscovered.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(hash);
            });

            File.WriteAllBytes(path, metadata);
            hooks.OnMetadataDiscovered = handler;

            metaget.Start(pipeline);
            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetafileDiscoveredWhenPopulated()
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

            metaget.Start(pipeline);
            hooks.OnMetadataDiscovered = handler;

            metaget.HandleMetadataMeasured(measured);
            metaget.HandleMetadataReceived(received);

            handler.Wait().Should().BeTrue();
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

            metaget.Start(pipeline);
            metaget.HandleMetadataMeasured(measured);

            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetadataPieceReceived()
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
                Piece = 1,
                Data = new byte[1024]
            };

            var handler = hooks.OnMetadataPieceReceived.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Peer.Should().NotBeNull();
                data.Piece.Should().Be(1);
            });

            hooks.OnMetadataPieceReceived = handler;

            metaget.Start(pipeline);
            metaget.HandleMetadataMeasured(measured);
            metaget.HandleMetadataReceived(received);

            handler.Wait().Should().BeTrue();
        }
    }
}