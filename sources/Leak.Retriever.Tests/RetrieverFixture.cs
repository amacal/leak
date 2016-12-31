using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Files;
using Leak.Glue;
using Leak.Memory;
using Leak.Metadata;
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
            Metainfo metainfo = null;
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

            RetrieverHooks hooks = new RetrieverHooks();
            RetrieverConfiguration configuration = new RetrieverConfiguration();

            Bitfield bitfield = new Bitfield(2);
            RetrieverService service = new RetrieverService(metainfo, destination, bitfield, glue, files, pipeline, hooks, configuration);

            return new RetrieverSession(service, data, metainfo.Hash, hooks);
        }

        public void Dispose()
        {
            pipeline.Stop();
            completion.Dispose();
        }
    }
}
