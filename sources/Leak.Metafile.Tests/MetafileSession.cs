using System;
using F2F.Sandbox;
using Leak.Common;

namespace Leak.Metafile.Tests
{
    public class MetafileSession : IDisposable
    {
        private readonly IFileSandbox sandbox;
        private readonly Metainfo metainfo;
        private readonly string path;
        private readonly byte[] data;
        private readonly MetafileService service;

        public MetafileSession(IFileSandbox sandbox, Metainfo metainfo, string path, byte[] data, MetafileService service)
        {
            this.sandbox = sandbox;
            this.metainfo = metainfo;
            this.path = path;
            this.data = data;
            this.service = service;
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

        public void Dispose()
        {
            sandbox.Dispose();
        }
    }
}
