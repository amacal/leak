using F2F.Sandbox;
using FluentAssertions;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Metaget;
using NUnit.Framework;
using System.IO;

namespace Leak.Core.Tests.Network
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

            hooks = new MetagetHooks();
            configuration = new MetagetConfiguration();
            metaget = new MetagetService(hash, sandbox.CreateDirectory($"{hash}"), hooks, configuration);

            worker.Start();
            pipeline.Start();
        }

        [TearDown]
        public void TearDown()
        {
            worker.Dispose();
            pipeline.Stop();
            sandbox.Dispose();
            metaget.Stop(pipeline);
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
    }
}