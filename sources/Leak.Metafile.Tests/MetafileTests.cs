using System.IO;
using FluentAssertions;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Metafile.Tests
{
    public class MetafileTests
    {
        [Test]
        public void ShouldTriggerMetafileVerifiedWhenCompleted()
        {
            using (MetafileFixture fixture = new MetafileFixture())
            using (MetafileSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileVerified, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Metainfo.Should().NotBeNull();
                    data.Metainfo.Hash.Should().Be(session.Hash);
                });

                File.WriteAllBytes(session.Path, session.Data);
                session.Service.Verify();

                handler.Wait().Should().BeTrue();
                session.Service.IsCompleted().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileVerifiedWhenWritten()
        {
            using (MetafileFixture fixture = new MetafileFixture())
            using (MetafileSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileVerified, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Metainfo.Should().NotBeNull();
                    data.Metainfo.Hash.Should().Be(session.Hash);
                });

                session.Service.Write(0, session.Data);
                handler.Wait().Should().BeTrue();

                session.Service.IsCompleted().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileWrittenWhenWritten()
        {
            using (MetafileFixture fixture = new MetafileFixture())
            using (MetafileSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileWritten, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Should().Be(1);
                    data.Size.Should().Be(16384);
                });

                session.Service.Write(1, new byte[16384]);
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerMetafileRejectedWhenWritten()
        {
            using (MetafileFixture fixture = new MetafileFixture())
            using (MetafileSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileRejected, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                });

                session.Service.Write(1, new byte[16384]);
                handler.Wait().Should().BeTrue();

                session.Service.IsCompleted().Should().BeFalse();
            }
        }

        [Test]
        public void ShouldTriggerMetafileRejectedWhenVerified()
        {
            using (MetafileFixture fixture = new MetafileFixture())
            using (MetafileSession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnMetafileRejected, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                });

                session.Service.Verify();
                handler.Wait().Should().BeTrue();

                session.Service.IsCompleted().Should().BeFalse();
            }
        }
    }
}