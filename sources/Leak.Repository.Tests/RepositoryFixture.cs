using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Files;
using Leak.Metadata;
using File = System.IO.File;

namespace Leak.Repository.Tests
{
    public class RepositoryFixture : IDisposable
    {
        private readonly CompletionThread completion;
        private readonly FileFactory files;

        public RepositoryFixture()
        {
            completion = new CompletionThread();
            completion.Start();

            files = new FileFactory(completion);
        }

        public RepositorySession Start()
        {
            Metainfo metainfo = null;
            RepositoryData data = new RepositoryData(20000);

            using (FileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, data.ToBytes());
                builder.AddFile(path);

                metainfo = builder.ToMetainfo();
            }

            FileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            string destination = Path.Combine(sandbox.Directory, metainfo.Hash.ToString());

            RepositoryHooks hooks = new RepositoryHooks();
            RepositoryConfiguration configuration = new RepositoryConfiguration();

            RepositoryService service = new RepositoryService(metainfo, destination, files, hooks, configuration);
            RepositorySession session = new RepositorySession(service, hooks, sandbox, metainfo.Hash, data);

            return session;
        }

        public void Dispose()
        {
            completion.Dispose();
        }
    }
}
