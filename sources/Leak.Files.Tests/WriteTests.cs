using F2F.Sandbox;
using Leak.Completion;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Leak.Files.Tests
{
    public class WriteTests
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
        public void CanWriteData()
        {
            byte[] buffer = new byte[20];
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                worker.Start();

                using (File file = factory.OpenOrCreate(path))
                {
                    file.Write(0, buffer, data =>
                    {
                        completed.Set();
                    });
                }

                using (FileStream stream = System.IO.File.OpenRead(path))
                {
                    stream.Read(new byte[10], 0, 10);
                }
            }

            Assert.That(completed.WaitOne(TimeSpan.FromSeconds(5)), Is.True);
        }
    }
}