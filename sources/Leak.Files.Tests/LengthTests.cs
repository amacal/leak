using F2F.Sandbox;
using Leak.Suckets;
using NUnit.Framework;
using System.IO;

namespace Leak.Files.Tests
{
    public class LengthTests
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
        public void CanProceedWithNewLength()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                using (File file = factory.OpenOrCreate(path))
                {
                    Assert.That(file.SetLength(10), Is.True);
                }
            }
        }

        [Test]
        public void CanSetNewLength()
        {
            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                using (File file = factory.OpenOrCreate(path))
                {
                    file.SetLength(10);
                }

                Assert.That(new FileInfo(path).Length, Is.EqualTo(10));
            }
        }
    }
}