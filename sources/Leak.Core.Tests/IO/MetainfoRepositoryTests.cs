using F2F.Sandbox;
using FluentAssertions;
using Leak.Core.IO;
using NUnit.Framework;
using System.IO;

namespace Leak.Core.Tests.IO
{
    [TestFixture]
    public class MetainfoRepositoryTests
    {
        private FileSandbox sandbox;

        [SetUp]
        public void SetUp()
        {
            sandbox = new FileSandbox(new EmptyFileLocator());
        }

        [TearDown]
        public void TearDown()
        {
            sandbox.Dispose();
        }

        [Test]
        public void RepositoryShouldNotHaveRandomHash()
        {
            byte[] hash = Bytes.Random(20);

            MetainfoRepository repository = new MetainfoRepository(with =>
            {
                with.Directory(sandbox.Directory);
            });

            repository.Contains(hash).Should().BeFalse();
        }

        [Test]
        public void RepositoryShouldContainRegisteredHash()
        {
            byte[] hash = Bytes.Random(20);

            MetainfoRepository repository = new MetainfoRepository(with =>
            {
                with.Directory(sandbox.Directory);
            });

            repository.Register(hash);
            repository.Contains(hash).Should().BeTrue();
        }

        [Test]
        public void RepositoryShouldContainIncludedMetainfo()
        {
            using (FileSandbox binaries = new FileSandbox(new EmptyFileLocator()))
            {
                string path = binaries.CreateFile("binary.data");
                File.WriteAllBytes(path, Bytes.Random(1024));

                MetainfoFile metainfo = new MetainfoFile(with =>
                {
                    with.Include(binaries.Directory);
                });

                MetainfoRepository repository = new MetainfoRepository(with =>
                {
                    with.Directory(sandbox.Directory);
                    with.Include(metainfo);
                });

                repository.Contains(metainfo.Hash).Should().BeTrue();
                repository.IsCompleted(metainfo.Hash).Should().BeTrue();
            }
        }

        [Test]
        public void RepositoryShouldProvideRequestedSinglePiece()
        {
            using (FileSandbox binaries = new FileSandbox(new EmptyFileLocator()))
            {
                string path = binaries.CreateFile("binary.data");
                File.WriteAllBytes(path, Bytes.Random(1024));

                MetainfoFile metainfo = new MetainfoFile(with =>
                {
                    with.Include(binaries.Directory);
                });

                MetainfoRepository repository = new MetainfoRepository(with =>
                {
                    with.Directory(sandbox.Directory);
                    with.Include(metainfo);
                });

                repository.GetData(metainfo.Hash, 0).Should().HaveCount(x => x < 16384);
                repository.GetData(metainfo.Hash, 0).Should().Equal(metainfo.Data);
            }
        }

        [Test]
        public void RepositoryShouldProvideRequestedMultiplePieces()
        {
            using (FileSandbox binaries = new FileSandbox(new EmptyFileLocator()))
            {
                string path = binaries.CreateFile("binary.data");
                File.WriteAllBytes(path, Bytes.Random(14 * 1024 * 1024));

                MetainfoFile metainfo = new MetainfoFile(with =>
                {
                    with.Include(binaries.Directory);
                });

                MetainfoRepository repository = new MetainfoRepository(with =>
                {
                    with.Directory(sandbox.Directory);
                    with.Include(metainfo);
                });

                repository.GetData(metainfo.Hash, 0).Should().HaveCount(16384);
                metainfo.Data.Should().StartWith(repository.GetData(metainfo.Hash, 0));
                metainfo.Data.Should().EndWith(repository.GetData(metainfo.Hash, 1));
            }
        }

        [Test]
        public void RepositoryShouldAcceptAllPiecesAndConsiderHashCompleted()
        {
            using (FileSandbox binaries = new FileSandbox(new EmptyFileLocator()))
            {
                string path = binaries.CreateFile("binary.data");
                File.WriteAllBytes(path, Bytes.Random(14 * 1024 * 1024));

                MetainfoFile metainfo = new MetainfoFile(with =>
                {
                    with.Include(binaries.Directory);
                });

                MetainfoRepository repository = new MetainfoRepository(with =>
                {
                    with.Directory(sandbox.Directory);
                });

                repository.Register(metainfo.Hash);
                repository.SetTotalSize(metainfo.Hash, metainfo.Data.Length);
                repository.SetData(metainfo.Hash, 0, metainfo.Data.ToBytes(0, 16384)).Should().BeFalse();
                repository.SetData(metainfo.Hash, 1, metainfo.Data.ToBytes(16384)).Should().BeTrue();
                repository.IsCompleted(metainfo.Hash).Should().BeTrue();
            }
        }
    }
}