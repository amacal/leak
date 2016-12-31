using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Metafile;

namespace Leak.Spartan.Tests
{
    public class SpartanSession : IDisposable
    {
        private readonly IFileSandbox sandbox;
        private readonly SpartanMeta meta;
        private readonly SpartanData data;
        private readonly SpartanService service;
        private readonly SpartanStage stage;
        private readonly MetafileService metafile;

        public SpartanSession(IFileSandbox sandbox, SpartanMeta meta, SpartanData data, SpartanService service, SpartanStage stage, MetafileService metafile)
        {
            this.sandbox = sandbox;
            this.meta = meta;
            this.data = data;
            this.service = service;
            this.stage = stage;
            this.metafile = metafile;
        }

        public SpartanService Service
        {
            get { return service; }
        }

        public SpartanHooks Hooks
        {
            get { return service.Hooks; }
        }

        public string Directory
        {
            get { return Path.Combine(sandbox.Directory, meta.Hash.ToString()); }
        }

        public FileHash Hash
        {
            get { return meta.Hash; }
        }

        public SpartanMeta Meta
        {
            get { return meta; }
        }

        public IFileSandbox Sandbox
        {
            get { return sandbox; }
        }

        public SpartanData Data
        {
            get { return data; }
        }

        public SpartanStage Stage
        {
            get { return stage; }
        }

        public void Dispose()
        {
            metafile.Dispose();
            service.Dispose();
            sandbox.Dispose();
        }
    }
}
