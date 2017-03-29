using System;
using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Data.Store;
using Leak.Files;
using Leak.Glue;
using Leak.Memory;
using Leak.Meta.Info;
using Leak.Tasks;
using File = System.IO.File;

namespace Leak.Data.Share.Tests
{
    public class DatashareFixture : IDisposable
    {
        private readonly CompletionThread completion;
        private readonly LeakPipeline pipeline;
        private readonly FileFactory files;

        public DatashareFixture()
        {
            completion = new CompletionThread();
            completion.Start();

            pipeline = new LeakPipeline();
            pipeline.Start();

            files = new FileFactory(completion);
        }

        public DatashareSession Start()
        {
            Metainfo metainfo;
            DatashareData data = new DatashareData(20000);

            IFileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            MetainfoBuilder builder = new MetainfoBuilder(sandbox.Directory);
            string path = sandbox.CreateFile("debian-8.5.0-amd64-CD-1.iso");

            File.WriteAllBytes(path, data.ToBytes());
            builder.AddFile(path);

            metainfo = builder.ToMetainfo();

            GlueService glue =
                new GlueBuilder()
                    .WithHash(metainfo.Hash)
                    .WithMemory(new MemoryBuilder().Build())
                    .Build();

            RepositoryService repository =
                new RepositoryBuilder()
                    .WithHash(metainfo.Hash)
                    .WithDestination(sandbox.Directory)
                    .WithFiles(files)
                    .WithPipeline(pipeline)
                    .Build();

            DatashareService datashare =
                new DatashareBuilder()
                    .WithHash(metainfo.Hash)
                    .WithGlue(glue)
                    .WithDataStore(repository)
                    .Build();

            return new DatashareSession(metainfo, data, sandbox, datashare, repository, glue);
        }

        public void Dispose()
        {
            completion.Dispose();
            pipeline.Stop();
        }
    }
}