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

        public SpartanSession Start(Goal tasks)
        {
            byte[] bytes;
            Metainfo metainfo = null;
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

            DataBlockFactory blocks = new BufferedBlockFactory();
            GlueService glue = new GlueFactory(blocks).Create(metainfo.Hash, new GlueHooks(), new GlueConfiguration());

            SpartanHooks hooks = new SpartanHooks();
            SpartanConfiguration configuration = new SpartanConfiguration { Tasks = tasks };

            SpartanService service = new SpartanService(pipeline, destination, glue, files, hooks, configuration);
            SpartanMeta meta = new SpartanMeta(metainfo.Hash, bytes);
            SpartanStage stage = new SpartanStage(hooks);

            return new SpartanSession(sandbox, meta, data, service, hooks, stage);
        }

        public void Dispose()
        {
            pipeline.Stop();
            completion.Dispose();
        }
    }
}
