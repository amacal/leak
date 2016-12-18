using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Metadata;

namespace Leak.Metafile.Tests
{
    public class MetafileFixture : IDisposable
    {
        public MetafileSession Start()
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

            MetafileHooks hooks = new MetafileHooks();
            MetafileService service = new MetafileService(metainfo.Hash, destination, hooks);

            return new MetafileSession(sandbox, metainfo, destination, data, service, hooks);
        }

        public void Dispose()
        {
        }
    }
}
