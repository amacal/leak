using F2F.Sandbox;
using Leak.Common;
using Leak.Metadata;
using System;
using FakeItEasy;
using Leak.Testing;
using File = System.IO.File;

namespace Leak.Spartan.Tests
{
    public class SpartanFixture : IDisposable
    {
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

            SpartanService spartan =
                new SpartanBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(new PipelineSimulator())
                    .WithMetaget(A.Fake<SpartanMetaget>())
                    .WithMetashare(A.Fake<SpartanMetashare>())
                    .WithDatastore(A.Fake<SpartanRepository>())
                    .WithDataget(A.Fake<SpartanRetriever>())
                    .WithDatashare(A.Fake<SpartanDatashare>())
                    .WithGoal(goal)
                    .Build();

            SpartanMeta meta = new SpartanMeta(metainfo.Hash, bytes);
            SpartanStage stage = new SpartanStage(spartan.Hooks);

            return new SpartanSession(metainfo, meta, data, spartan, stage);
        }

        public void Dispose()
        {
        }
    }
}