using System;
using System.IO;
using System.Threading.Tasks;
using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Files;
using Leak.Metadata;
using Leak.Tasks;
using File = System.IO.File;

namespace Leak.Meta.Store.Tests
{
    public class MetafileFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread completion;
        private readonly FileFactory files;

        public MetafileFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            completion = new CompletionThread();
            completion.Start();

            files = new FileFactory(completion);
        }

        public MetafileSession Start(bool completed = false)
        {
            Metainfo metainfo = null;
            byte[] data = null;

            using (IFileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, Bytes.Random(20000));
                builder.AddFile(path);

                metainfo = builder.ToMetainfo(out data);
            }

            IFileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            string destination = Path.Combine(sandbox.Directory, metainfo.Hash.ToString());

            if (completed)
            {
                File.WriteAllBytes(destination, data);
            }

            MetafileService service =
                new MetafileBuilder()
                    .WithHash(metainfo.Hash)
                    .WithDestination(destination)
                    .WithFiles(files)
                    .WithPipeline(pipeline)
                    .Build();

            TaskCompletionSource<bool> onVerified = new TaskCompletionSource<bool>();

            service.Hooks.OnMetafileVerified += _ => onVerified.SetResult(true);

            return new MetafileSession(sandbox, metainfo, destination, data, service, onVerified.Task);
        }

        public void Dispose()
        {
            completion.Dispose();
            pipeline.Stop();
        }
    }
}