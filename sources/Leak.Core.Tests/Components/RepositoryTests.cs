using FluentAssertions;
using Leak.Common;
using Leak.Core.Repository;
using Leak.Core.Tests.Core;
using NUnit.Framework;
using System.IO;

namespace Leak.Core.Tests.Components
{
    public class RepositoryTests
    {
        private RepositoryHooks hooks;
        private RepositoryConfiguration configuration;

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

            configuration = new RepositoryConfiguration();
            hooks = new RepositoryHooks();
        }

        [TearDown]
        public void TearDown()
        {
            environemnt.Dispose();
            environemnt = null;
        }

        private RepositoryService NewRepositoryService()
        {
            return new RepositoryService(fixture.Debian.Metadata.Metainfo, environemnt.Destination, environemnt.Files, hooks, configuration);
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

            using (RepositoryService repository = NewRepositoryService())
            {
                repository.Start();
                repository.Verify(new Bitfield(fixture.Debian.Binary.Pieces.Length));

                handler.Wait().Should().BeTrue();
            }

            environemnt.Sandbox.ExistsFile(file).Should().BeTrue();
        }

        [Test]
        public void ShouldTriggerDataVerifiedWithEmptyBitfield()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnDataVerified, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Bitfield.Should().NotBeNull();
                data.Bitfield.Completed.Should().Be(0);
                data.Bitfield.Length.Should().Be(fixture.Debian.Binary.Pieces.Length);
            });

            using (RepositoryService repository = NewRepositoryService())
            {
                repository.Start();
                repository.Verify(new Bitfield(fixture.Debian.Binary.Pieces.Length));

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataVerifiedWithCompletedBitfield()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnDataVerified, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Bitfield.Should().NotBeNull();
                data.Bitfield.Completed.Should().Be(fixture.Debian.Binary.Pieces.Length);
                data.Bitfield.Length.Should().Be(fixture.Debian.Binary.Pieces.Length);
            });

            using (RepositoryService repository = NewRepositoryService())
            {
                repository.Start();

                for (int i = 0; i < fixture.Debian.Binary.Pieces.Length; i++)
                {
                    repository.Write(fixture.Debian.Binary.Pieces[i].Blocks[0].Data);
                }

                repository.Verify(new Bitfield(fixture.Debian.Binary.Pieces.Length));
                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataWritten()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnBlockWritten, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Piece.Should().Be(0);
                data.Block.Should().Be(0);
                data.Size.Should().Be(16384);
            });

            using (RepositoryService repository = NewRepositoryService())
            {
                repository.Start();
                repository.Write(fixture.Debian.Binary.Pieces[0].Blocks[0].Data);

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataAccepted()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnPieceAccepted, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Piece.Should().Be(0);
            });

            using (RepositoryService repository = NewRepositoryService())
            {
                repository.Start();
                repository.Write(fixture.Debian.Binary.Pieces[0].Blocks[0].Data);
                repository.Verify(new PieceInfo(0));

                handler.Wait().Should().BeTrue();
            }
        }

        [Test]
        public void ShouldTriggerDataRejected()
        {
            Trigger handler = Trigger.Bind(ref hooks.OnPieceRejected, data =>
            {
                data.Hash.Should().Be(fixture.Debian.Metadata.Hash);
                data.Piece.Should().Be(0);
            });

            using (RepositoryService repository = NewRepositoryService())
            {
                repository.Start();
                repository.Verify(new PieceInfo(0));

                handler.Wait().Should().BeTrue();
            }
        }
    }
}