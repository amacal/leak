using System.IO;
using FluentAssertions;
using Leak.Common;
using Leak.Testing;
using NUnit.Framework;

namespace Leak.Datastore.Tests
{
    public class RepositoryTests
    {
        [Test]
        public void ShouldTriggerDataAllocated()
        {
            using (RepositoryFixture fixture = new RepositoryFixture())
            using (RepositorySession session = fixture.Start())
            {
                string path = Path.Combine(session.Sandbox.Directory, session.Hash.ToString());
                string file = Path.Combine(path, "debian-8.5.0-amd64-CD-1.iso");

                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataAllocated, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Directory.Should().EndWith(path);
                });

                session.Service.Start();
                session.Service.HandleMetadataDiscovered(session.Metainfo);
                session.Service.Verify(new Bitfield(session.Data.Pieces));

                handler.Wait().Should().BeTrue();
                session.Sandbox.ExistsFile(file).Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataVerifiedWithEmptyBitfield()
        {
            using (RepositoryFixture fixture = new RepositoryFixture())
            using (RepositorySession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataVerified, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Bitfield.Should().NotBeNull();
                    data.Bitfield.Completed.Should().Be(0);
                    data.Bitfield.Length.Should().Be(session.Data.Pieces);
                });

                session.Service.Start();
                session.Service.HandleMetadataDiscovered(session.Metainfo);
                session.Service.Verify(new Bitfield(session.Data.Pieces));

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataVerifiedWithCompletedBitfield()
        {
            using (RepositoryFixture fixture = new RepositoryFixture())
            using (RepositorySession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnDataVerified, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Bitfield.Should().NotBeNull();
                    data.Bitfield.Completed.Should().Be(session.Data.Pieces);
                    data.Bitfield.Length.Should().Be(session.Data.Pieces);
                });

                session.Service.Start();
                session.Service.HandleMetadataDiscovered(session.Metainfo);

                session.Service.Write(0, session.Data[0]);
                session.Service.Write(1, session.Data[1]);

                session.Service.Verify(new Bitfield(session.Data.Pieces));
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerBlockWritten()
        {
            using (RepositoryFixture fixture = new RepositoryFixture())
            using (RepositorySession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnBlockWritten, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Block.Piece.Index.Should().Be(1);
                    data.Block.Offset.Should().Be(0);
                    data.Block.Size.Should().Be(session.Data[1].Length);
                });

                session.Service.Start();
                session.Service.HandleMetadataDiscovered(session.Metainfo);
                session.Service.Write(1, session.Data[1]);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerBlockRead()
        {
            using (RepositoryFixture fixture = new RepositoryFixture())
            using (RepositorySession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnBlockRead, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Block.Piece.Index.Should().Be(1);
                    data.Block.Offset.Should().Be(0);
                    data.Block.Size.Should().Be(session.Data[1].Length);
                });

                session.Service.Start();
                session.Service.HandleMetadataDiscovered(session.Metainfo);
                session.Service.Write(1, session.Data[1]);
                session.Service.Read(1, session.Data[1].Length);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataAccepted()
        {
            using (RepositoryFixture fixture = new RepositoryFixture())
            using (RepositorySession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnPieceAccepted, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Index.Should().Be(1);
                });

                session.Service.Start();
                session.Service.HandleMetadataDiscovered(session.Metainfo);

                session.Service.Write(1, session.Data[1]);
                session.Service.Verify(new PieceInfo(1));

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataRejected()
        {
            using (RepositoryFixture fixture = new RepositoryFixture())
            using (RepositorySession session = fixture.Start())
            {
                Trigger handler = Trigger.Bind(ref session.Hooks.OnPieceRejected, data =>
                {
                    data.Hash.Should().Be(session.Hash);
                    data.Piece.Index.Should().Be(1);
                });

                session.Service.Start();
                session.Service.HandleMetadataDiscovered(session.Metainfo);
                session.Service.Verify(new PieceInfo(1));

                handler.Wait().Should().BeTrue();
            }
        }
    }
}