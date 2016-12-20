using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Glue;
using Leak.Memory;
using Leak.Metadata;
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
            DataBlockFactory blocks = new BufferedBlockFactory();

            MetagetHooks hooks = new MetagetHooks();
            MetagetConfiguration configuration = new MetagetConfiguration();

            GlueService glue = new GlueFactory(blocks).Create(metainfo.Hash, new GlueHooks(), new GlueConfiguration());
            MetagetService service = new MetagetService(pipeline, glue, destination, hooks, configuration);

            return new MetagetSession(sandbox, destination + ".metainfo", metainfo.Hash, data, service, hooks);
        }

        public void Dispose()
        {
            pipeline.Stop();
        }
    }
}
