using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Data.Store;
using Leak.Files;
using NUnit.Framework;

namespace Leak.Core.Tests.Repository
{
    public class CacheTests
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
            sandbox?.Dispose();
            sandbox = null;
        }

        [Test]
        public void CanFindFirstPiece()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024) };

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    RepositoryViewEntry[] result = cache.Find(0);

                    Assert.That(result, Has.Length.EqualTo(1));
                }
            }
        }

        [Test]
        public void CanHandlePieceWithFileStartingAtLastByte()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 16383), new MetainfoEntry("file-1.bin", 1024) };

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    RepositoryViewEntry[] result0 = cache.Find(0);
                    RepositoryViewEntry[] result1 = cache.Find(1);

                    Assert.That(result0, Has.Length.EqualTo(2));
                    Assert.That(result1, Has.Length.EqualTo(1));
                }
            }
        }

        [Test]
        public void CanHandlePieceWithFileAlignedToPieceSize()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 16384), new MetainfoEntry("file-2.bin", 1024) };

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    RepositoryViewEntry[] result0 = cache.Find(0);
                    RepositoryViewEntry[] result1 = cache.Find(1);

                    Assert.That(result0, Has.Length.EqualTo(1));
                    Assert.That(result1, Has.Length.EqualTo(1));
                }
            }
        }

        [Test]
        public void CanHandlePieceWithFileEndingAtFirstByte()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 16385), new MetainfoEntry("file-2.bin", 1024) };

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    RepositoryViewEntry[] result0 = cache.Find(0);
                    RepositoryViewEntry[] result1 = cache.Find(1);

                    Assert.That(result0, Has.Length.EqualTo(1));
                    Assert.That(result1, Has.Length.EqualTo(2));
                }
            }
        }
    }
}