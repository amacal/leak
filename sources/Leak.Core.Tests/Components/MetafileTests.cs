using F2F.Sandbox;
using FluentAssertions;
using Leak.Common;
using Leak.Core.Metafile;
using Leak.Core.Tests.Core;
using NUnit.Framework;
using System.IO;

namespace Leak.Core.Tests.Components
{
    public class MetafileTests
    {
        private FileHash hash;
        private MetafileService metafile;
        private MetafileHooks hooks;
        private FileSandbox sandbox;

        private byte[] metadata;
        private string path;

        [SetUp]
        public void SetUp()
        {
            Fixture.Sample(out hash, out metadata);

            sandbox = Sandbox.New();
            path = sandbox.CreateFile($"{hash}.metainfo");

            hooks = new MetafileHooks();
            metafile = new MetafileService(hash, path, hooks);
        }

        [TearDown]
        public void TearDown()
        {
            sandbox.Dispose();
        }

        [Test]
        public void ShouldTriggerMetafileVerifiedWhenCompleted()
        {
            var handler = hooks.OnMetafileVerified.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(hash);
            });

            hooks.OnMetafileVerified = handler;
            File.WriteAllBytes(path, metadata);

            metafile.Verify();
            handler.Wait().Should().BeTrue();

            metafile.IsCompleted().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetafileVerifiedWhenWritten()
        {
            var handler = hooks.OnMetafileVerified.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Metainfo.Should().NotBeNull();
                data.Metainfo.Hash.Should().Be(hash);
            });

            hooks.OnMetafileVerified = handler;

            metafile.Write(0, metadata);
            handler.Wait().Should().BeTrue();

            metafile.IsCompleted().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetafileWrittenWhenWritten()
        {
            var handler = hooks.OnMetafileWritten.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
                data.Piece.Should().Be(1);
                data.Size.Should().Be(16384);
            });

            hooks.OnMetafileWritten = handler;

            metafile.Write(1, new byte[16384]);
            handler.Wait().Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerMetafileRejectedWhenWritten()
        {
            var handler = hooks.OnMetafileRejected.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
            });

            hooks.OnMetafileRejected = handler;

            metafile.Write(1, new byte[16384]);
            handler.Wait().Should().BeTrue();

            metafile.IsCompleted().Should().BeFalse();
        }

        [Test]
        public void ShouldTriggerMetafileRejectedWhenVerified()
        {
            var handler = hooks.OnMetafileRejected.Trigger(data =>
            {
                data.Hash.Should().Be(hash);
            });

            hooks.OnMetafileRejected = handler;

            metafile.Verify();
            handler.Wait().Should().BeTrue();

            metafile.IsCompleted().Should().BeFalse();
        }
    }
}