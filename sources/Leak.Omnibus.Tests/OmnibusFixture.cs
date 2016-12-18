using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Metadata;
using Leak.Tasks;

namespace Leak.Omnibus.Tests
{
    public class OmnibusFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;

        public OmnibusFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();
        }

        public OmnibusSession Start()
        {
            Metainfo metainfo;
            byte[] data = Bytes.Random(20000);

            using (FileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, data);
                builder.AddFile(path);

                metainfo = builder.ToMetainfo();
            }

            OmnibusHooks hooks = new OmnibusHooks();
            OmnibusConfiguration configuration = new OmnibusConfiguration();

            Bitfield bitfield = new Bitfield(metainfo.Pieces.Length);
            OmnibusService service = new OmnibusService(metainfo, bitfield, pipeline, hooks, configuration);

            service.Start();

            return new OmnibusSession(metainfo.Hash, service, hooks);
        }

        public void Dispose()
        {
            pipeline.Stop();
        }
    }
}
