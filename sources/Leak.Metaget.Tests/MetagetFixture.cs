using F2F.Sandbox;
using FakeItEasy;
using Leak.Common;
using Leak.Metadata;
using Leak.Testing;
using System;
using System.IO;
using File = System.IO.File;

namespace Leak.Metaget.Tests
{
    public class MetagetFixture : IDisposable
    {
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

            MetagetService metaget =
                new MetagetBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(new PipelineSimulator())
                    .WithGlue(A.Fake<MetagetGlue>())
                    .WithMetafile(A.Fake<MetagetMetafile>())
                    .Build();

            return new MetagetSession(sandbox, metainfo.Hash, data, metaget);
        }

        public void Dispose()
        {
        }
    }
}