using F2F.Sandbox;
using NUnit.Framework;
using System.IO;
using Leak.Completion;

namespace Leak.Files.Tests
{
    public class FactoryTests
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
        public void CanCreateNewFile()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                using (File file = factory.OpenOrCreate(path))
                {
                }

                Assert.That(System.IO.File.Exists(path), Is.True);
                Assert.That(new FileInfo(path).Length, Is.Zero);
            }
        }

        [Test]
        public void CanOpenExistingFile()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                using (FileStream stream = System.IO.File.Create(path))
                {
                    stream.Write(new byte[10], 0, 10);
                    stream.Flush(true);
                }

                using (File file = factory.OpenOrCreate(path))
                {
                }

                Assert.That(System.IO.File.Exists(path), Is.True);
                Assert.That(new FileInfo(path).Length, Is.EqualTo(10));
            }
        }

        [Test]
        public void CanFailOnNotFoundFile()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                using (File file = factory.Open(path))
                {
                    Assert.That(file, Is.Null);
                }

                Assert.That(System.IO.File.Exists(path), Is.False);
            }
        }

        [Test]
        public void CanCreateRussianFile()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "абг.txt");
                FileFactory factory = new FileFactory(worker);

                using (File file = factory.OpenOrCreate(path))
                {
                }

                Assert.That(System.IO.File.Exists(path), Is.True);
                Assert.That(new FileInfo(path).Length, Is.Zero);
            }
        }
    }
}