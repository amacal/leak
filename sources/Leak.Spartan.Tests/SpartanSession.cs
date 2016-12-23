using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;

namespace Leak.Spartan.Tests
{
    public class SpartanSession : IDisposable
    {
        private readonly IFileSandbox sandbox;
        private readonly SpartanMeta meta;
        private readonly SpartanData data;
        private readonly SpartanService service;
        private readonly SpartanHooks hooks;
        private readonly SpartanStage stage;

        public SpartanSession(IFileSandbox sandbox, SpartanMeta meta, SpartanData data, SpartanService service, SpartanHooks hooks, SpartanStage stage)
        {
            this.sandbox = sandbox;
            this.meta = meta;
            this.data = data;
            this.service = service;
            this.hooks = hooks;
            this.stage = stage;
        }

        public SpartanService Service
        {
            get { return service; }
        }

        public SpartanHooks Hooks
        {
            get { return hooks; }
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
            service.Dispose();
            sandbox.Dispose();
        }
    }
}
