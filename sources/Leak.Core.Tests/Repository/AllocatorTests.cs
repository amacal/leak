using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Core.Repository;
using Leak.Files;
using NUnit.Framework;
using System.IO;
using File = System.IO.File;

namespace Leak.Core.Tests.Repository
{
    public class AllocatorTests
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
        public void CanDealWithoutEntries()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = new MetainfoEntry[0];

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    Assert.That(cache, Is.Not.Null);
                }
            }
        }

        [Test]
        public void CanDealWithoutFiles()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024) };

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    Assert.That(cache, Is.Not.Null);
                }
            }
        }

        [Test]
        public void CanDealWithExistingFiles()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024) };
            File.Create(Path.Combine(directory, entries[0].Name[0])).Dispose();

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    Assert.That(cache, Is.Not.Null);
                }
            }
        }
    }
}