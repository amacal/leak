using F2F.Sandbox;
using Leak.Common;
using System;
using System.Threading.Tasks;

namespace Leak.Metafile.Tests
{
    public class MetafileSession : IDisposable
    {
        private readonly IFileSandbox sandbox;
        private readonly Metainfo metainfo;
        private readonly string path;
        private readonly byte[] data;
        private readonly MetafileService service;
        private readonly Task onVerified;

        public MetafileSession(IFileSandbox sandbox, Metainfo metainfo, string path, byte[] data, MetafileService service, Task onVerified)
        {
            this.sandbox = sandbox;
            this.metainfo = metainfo;
            this.path = path;
            this.data = data;
            this.service = service;
            this.onVerified = onVerified;
        }

        public MetafileHooks Hooks
        {
            get { return service.Hooks; }
        }

        public MetafileService Service
        {
            get { return service; }
        }

        public FileHash Hash
        {
            get { return metainfo.Hash; }
        }

        public string Path
        {
            get { return path; }
        }

        public byte[] Data
        {
            get { return data; }
        }

        public Task Verified
        {
            get { return onVerified; }
        }

        public void Dispose()
        {
            service.Dispose();
            sandbox.Dispose();
        }
    }
}