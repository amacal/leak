using System;
using System.IO;
using System.Threading.Tasks;
using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Extensions.Metadata;
using Leak.Files;
using Leak.Glue;
using Leak.Memory;
using Leak.Meta.Info;
using Leak.Meta.Store;
using Leak.Tasks;
using File = System.IO.File;

namespace Leak.Meta.Share.Tests
{
    public class MetashareFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread completion;
        private readonly FileFactory files;

        public MetashareFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            completion = new CompletionThread();
            completion.Start();

            files = new FileFactory(completion);
        }

        public MetashareSession Start()
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

            File.WriteAllBytes(destination + ".metainfo", bytes);

            GlueService glue =
                new GlueBuilder()
                    .WithHash(metainfo.Hash)
                    .WithMemory(new MemoryBuilder().Build())
                    .WithPlugin(new MetadataPlugin(new MetadataHooks()))
                    .Build();

            MetafileService metafile =
                new MetafileBuilder()
                    .WithHash(metainfo.Hash)
                    .WithDestination(destination + ".metainfo")
                    .WithFiles(files)
                    .WithPipeline(pipeline)
                    .Build();

            MetashareService service =
                new MetashareBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(pipeline)
                    .WithGlue(glue)
                    .WithMetafile(metafile)
                    .Build();

            TaskCompletionSource<bool> onVerified = new TaskCompletionSource<bool>();
            metafile.Hooks.OnMetafileVerified += _ => onVerified.SetResult(true);

            metafile.Start();
            metafile.Verify();
            onVerified.Task.Wait(5000);

            return new MetashareSession(service);
        }

        public void Dispose()
        {
            completion.Dispose();
            pipeline.Stop();
        }
    }
}