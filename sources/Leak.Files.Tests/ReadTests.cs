using F2F.Sandbox;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using Leak.Completion;

namespace Leak.Files.Tests
{
    public class ReadTests
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
        public void CanReadData()
        {
            byte[] buffer = new byte[20];
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                using (FileStream stream = System.IO.File.Create(path))
                {
                    stream.Write(new byte[10], 0, 10);
                    stream.Flush(true);

                    worker.Start();
                }

                using (File file = factory.OpenOrCreate(path))
                {
                    file.Read(0, buffer, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(10));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(TimeSpan.FromSeconds(5)), Is.True);
                }
            }
        }

        [Test]
        public void CanReadDataAtTheEndOfFile()
        {
            byte[] buffer = new byte[20];
            ManualResetEvent completed = new ManualResetEvent(false);

            using (CompletionThread worker = new CompletionThread())
            {
                string path = Path.Combine(sandbox.Directory, "abc.txt");
                FileFactory factory = new FileFactory(worker);

                using (FileStream stream = System.IO.File.Create(path))
                {
                    stream.Write(new byte[10], 0, 10);
                    stream.Flush(true);

                    worker.Start();
                }

                using (File file = factory.OpenOrCreate(path))
                {
                    file.Read(10, buffer, data =>
                    {
                        Assert.That(data.Count, Is.EqualTo(0));

                        completed.Set();
                    });

                    Assert.That(completed.WaitOne(TimeSpan.FromSeconds(5)), Is.True);
                }
            }
        }
    }
}