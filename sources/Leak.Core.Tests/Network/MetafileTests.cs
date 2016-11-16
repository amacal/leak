using F2F.Sandbox;
using FluentAssertions;
using Leak.Core.Common;
using Leak.Core.Metadata;
using Leak.Core.Metafile;
using NUnit.Framework;

namespace Leak.Core.Tests.Network
{
    public class MetafileTests
    {
        private FileHash hash;
        private MetafileService metafile;
        private MetafileHooks hooks;
        private FileSandbox sandbox;

        [SetUp]
        public void SetUp()
        {
            byte[] data;
            string path;

            using (FileSandbox forBuilder = Sandbox.New())
            {
                MetainfoBuilder builder = new MetainfoBuilder(forBuilder.Directory);
                string file = forBuilder.CreateFile("abc.txt");

                builder.AddFile(file);

                hash = builder.GetHash();
                data = builder.ToBytes();
            }

            sandbox = Sandbox.New();
            path = sandbox.CreateFile($"{hash}.metainfo", data);

            hooks = new MetafileHooks();
            metafile = new MetafileService(hash, path, hooks);
        }

        [TearDown]
        public void TearDown()
        {
            sandbox.Dispose();
        }

        [Test]
        public void ShouldTriggerMetafileDiscoveredWhenCompleted()
        {
            var handler = hooks.OnMetadataDiscovered.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(hash);
            });

            hooks.OnMetadataDiscovered = handler;

            metafile.Verify();
            handler.Wait().Should().BeTrue();
        }
    }
}