using System;
using F2F.Sandbox;
using Leak.Common;
using Leak.Core.Metafile;

namespace Leak.Metafile.Tests
{
    public class MetafileSession : IDisposable
    {
        private readonly IFileSandbox sandbox;
        private readonly Metainfo metainfo;
        private readonly string path;
        private readonly byte[] data;
        private readonly MetafileService service;
        private readonly MetafileHooks hooks;

        public MetafileSession(IFileSandbox sandbox, Metainfo metainfo, string path, byte[] data, MetafileService service, MetafileHooks hooks)
        {
            this.sandbox = sandbox;
            this.metainfo = metainfo;
            this.path = path;
            this.data = data;
            this.service = service;
            this.hooks = hooks;
        }

        public MetafileHooks Hooks
        {
            get { return hooks; }
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
