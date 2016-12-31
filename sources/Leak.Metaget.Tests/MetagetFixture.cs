using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Glue;
using Leak.Memory;
using Leak.Metadata;
using Leak.Metafile;
using Leak.Tasks;

namespace Leak.Metaget.Tests
{
    public class MetagetFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;

        public MetagetFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();
        }

        public MetagetSession Start()
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

            GlueService glue =
                new GlueBuilder()
                    .WithHash(metainfo.Hash)
                    .WithBlocks(new BufferedBlockFactory())
                    .Build();

            MetafileService metafile =
                new MetafileBuilder()
                    .WithHash(metainfo.Hash)
                    .WithDestination(destination + ".metainfo")
                    .Build();

            MetagetService metaget =
                new MetagetBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(pipeline)
                    .WithGlue(glue)
                    .WithMetafile(metafile)
                    .Build();

            return new MetagetSession(sandbox, destination + ".metainfo", metainfo.Hash, data, metaget);
        }

        public void Dispose()
        {
            pipeline.Stop();
        }
    }
}
