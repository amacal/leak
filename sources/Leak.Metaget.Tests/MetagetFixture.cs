using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Files;
using Leak.Glue;
using Leak.Memory;
using Leak.Metadata;
using Leak.Metafile;
using Leak.Tasks;
using File = System.IO.File;

namespace Leak.Metaget.Tests
{
    public class MetagetFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread completion;
        private readonly FileFactory files;

        public MetagetFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            completion = new CompletionThread();
            completion.Start();

            files = new FileFactory(completion);
        }

        public MetagetSession Start(bool completed = false)
        {
            Metainfo metainfo;
            byte[] bytes = Bytes.Random(20000);

            using (FileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, bytes);
                builder.AddFile(path);

                metainfo = builder.ToMetainfo(out bytes);
            }

            FileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            string destination = Path.Combine(sandbox.Directory, metainfo.Hash.ToString());

            MetagetData data = new MetagetData(bytes);

            if (completed)
            {
                File.WriteAllBytes(destination + ".metainfo", data.ToBytes());
            }

            GlueService glue =
                new GlueBuilder()
                    .WithHash(metainfo.Hash)
                    .WithBlocks(new BufferedBlockFactory())
                    .Build();

            MetafileService metafile =
                new MetafileBuilder()
                    .WithHash(metainfo.Hash)
                    .WithDestination(destination + ".metainfo")
                    .WithFiles(files)
                    .WithPipeline(pipeline)
                    .Build();

            MetagetService metaget =
                new MetagetBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(pipeline)
                    .WithGlue(glue)
                    .WithMetafile(metafile)
                    .Build();

            metafile.Start();

            return new MetagetSession(sandbox, destination + ".metainfo", metainfo.Hash, data, metaget, metafile);
        }

        public void Dispose()
        {
            completion.Dispose();
            pipeline.Stop();
        }
    }
}
