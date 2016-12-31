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
using Leak.Metaget;
using Leak.Tasks;
using File = System.IO.File;

namespace Leak.Spartan.Tests
{
    public class SpartanFixture : IDisposable
    {
        private readonly CompletionThread completion;
        private readonly LeakPipeline pipeline;
        private readonly FileFactory files;

        public SpartanFixture()
        {
            completion = new CompletionThread();
            completion.Start();

            pipeline = new LeakPipeline();
            pipeline.Start();

            files = new FileFactory(completion);
        }

        public SpartanSession Start(Goal goal)
        {
            byte[] bytes;
            Metainfo metainfo;
            SpartanData data = new SpartanData(20000);

            using (FileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, data.ToBytes());
                builder.AddFile(path);

                metainfo = builder.ToMetainfo(out bytes);
            }

            FileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            string destination = Path.Combine(sandbox.Directory, metainfo.Hash.ToString());

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

            SpartanService spartan = 
                new SpartanBuilder()
                    .WithHash(metainfo.Hash)
                    .WithDestination(destination)
                    .WithPipeline(pipeline)
                    .WithFiles(files)
                    .WithGlue(glue)
                    .WithMetaget(metaget)
                    .WithGoal(goal)
                    .Build();

            SpartanMeta meta = new SpartanMeta(metainfo.Hash, bytes);
            SpartanStage stage = new SpartanStage(spartan.Hooks);

            metafile.Start();

            return new SpartanSession(sandbox, meta, data, spartan, stage, metafile);
        }

        public void Dispose()
        {
            pipeline.Stop();
            completion.Dispose();
        }
    }
}
