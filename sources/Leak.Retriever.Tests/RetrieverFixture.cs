using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Events;
using Leak.Files;
using Leak.Glue;
using Leak.Memory;
using Leak.Metadata;
using Leak.Omnibus;
using Leak.Repository;
using Leak.Tasks;
using File = System.IO.File;

namespace Leak.Retriever.Tests
{
    public class RetrieverFixture : IDisposable
    {
        private readonly CompletionThread completion;
        private readonly LeakPipeline pipeline;
        private readonly FileFactory files;

        public RetrieverFixture()
        {
            completion = new CompletionThread();
            completion.Start();

            pipeline = new LeakPipeline();
            pipeline.Start();

            files = new FileFactory(completion);
        }

        public RetrieverSession Start()
        {
            Metainfo metainfo;
            RetrieverData data = new RetrieverData(20000);

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

            GlueService glue =
                new GlueBuilder()
                    .WithHash(metainfo.Hash)
                    .WithBlocks(new BufferedBlockFactory())
                    .Build();

            RepositoryService repository =
                new RepositoryBuilder()
                    .WithHash(metainfo.Hash)
                    .WithDestination(destination)
                    .WithFiles(files)
                    .Build();

            OmnibusService omnibus =
                new OmnibusBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(pipeline)
                    .Build();

            RetrieverService service =
                new RetrieverBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(pipeline)
                    .WithGlue(glue)
                    .WithRepository(repository)
                    .WithOmnibus(omnibus)
                    .Build();

            repository.Start();
            omnibus.Start();

            omnibus.Handle(new MetadataDiscovered
            {
                Hash = metainfo.Hash,
                Metainfo = metainfo
            });

            repository.Handle(new MetadataDiscovered
            {
                Hash = metainfo.Hash,
                Metainfo = metainfo
            });

            omnibus.Handle(new DataVerified
            {
                Hash = metainfo.Hash,
                Bitfield = new Bitfield(metainfo.Pieces.Length)
            });

            return new RetrieverSession(service, data);
        }

        public void Dispose()
        {
            pipeline.Stop();
            completion.Dispose();
        }
    }
}
