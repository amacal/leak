using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Files;
using Leak.Repository;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace Leak.Core.Tests.Repository
{
    public class ViewTests
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
        public void CanReadPieceFromOneFile()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024) };

            TimeSpan timeout = TimeSpan.FromSeconds(5);
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                worker.Start();

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    byte[] buffer = new byte[16384];
                    RepositoryView view = new RepositoryView(cache);

                    view.Read(buffer, 0, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(1024));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(timeout), Is.True);
                }
            }
        }

        [Test]
        public void CanReadBlockFromOneFile()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024) };

            TimeSpan timeout = TimeSpan.FromSeconds(5);
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                worker.Start();

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    byte[] buffer = new byte[16384];
                    RepositoryView view = new RepositoryView(cache);

                    view.Read(buffer, 0, 0, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(1024));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(timeout), Is.True);
                }
            }
        }

        [Test]
        public void CanWritePieceToOneFile()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024) };

            TimeSpan timeout = TimeSpan.FromSeconds(5);
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);
                byte[] buffer = Bytes.Random(1024);

                worker.Start();

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    RepositoryView view = new RepositoryView(cache);

                    view.Write(buffer, 0, 0, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(1024));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(timeout), Is.True);
                }

                byte[] source = System.IO.File.ReadAllBytes(System.IO.Path.Combine(directory, "file-1.bin"));
                Assert.That(buffer, Is.EqualTo(source));
            }
        }

        [Test]
        public void CanWriteDataToSecondBlockInSecondFile()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024), new MetainfoEntry("file-2.bin", 16384) };

            TimeSpan timeout = TimeSpan.FromSeconds(5);
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);
                byte[] buffer = Bytes.Random(1024);

                worker.Start();

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 2 * 16384, 16384))
                {
                    RepositoryView view = new RepositoryView(cache);

                    view.Write(buffer, 0, 1, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(1024));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(timeout), Is.True);
                }

                byte[] source = System.IO.File.ReadAllBytes(System.IO.Path.Combine(directory, "file-2.bin"));
                Assert.That(buffer, Is.EqualTo(source.Skip(16384 - 1024)));
            }
        }

        [Test]
        public void CanReadPieceFromTwoFiles()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 1024), new MetainfoEntry("file-2.bin", 1024) };

            TimeSpan timeout = TimeSpan.FromSeconds(5);
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                worker.Start();

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    byte[] buffer = new byte[16384];
                    RepositoryView view = new RepositoryView(cache);

                    view.Read(buffer, 0, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(2048));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(timeout), Is.True);
                }
            }
        }

        [Test]
        public void CanReadPieceFromLargeFile()
        {
            string directory = sandbox.Directory;
            MetainfoEntry[] entries = { new MetainfoEntry("file-1.bin", 102400) };

            TimeSpan timeout = TimeSpan.FromSeconds(5);
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                FileFactory factory = new FileFactory(worker);
                RepositoryViewAllocator allocator = new RepositoryViewAllocator(factory);

                worker.Start();

                using (RepositoryViewCache cache = allocator.Allocate(directory, entries, 16384, 16384))
                {
                    byte[] buffer = new byte[16384];
                    RepositoryView view = new RepositoryView(cache);

                    view.Read(buffer, 0, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(16384));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(timeout), Is.True);
                }
            }
        }
    }
}