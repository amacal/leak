using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Metafile;
using Leak.Repository;

namespace Leak.Spartan.Tests
{
    public class SpartanSession : IDisposable
    {
        private readonly Metainfo metainfo;
        private readonly IFileSandbox sandbox;
        private readonly SpartanMeta meta;
        private readonly SpartanData data;
        private readonly SpartanService spartan;
        private readonly SpartanStage stage;
        private readonly MetafileService metafile;
        private readonly RepositoryService repository;

        public SpartanSession(Metainfo metainfo, IFileSandbox sandbox, SpartanMeta meta, SpartanData data, SpartanService spartan, SpartanStage stage, MetafileService metafile, RepositoryService repository)
        {
            this.metainfo = metainfo;
            this.sandbox = sandbox;
            this.meta = meta;
            this.data = data;
            this.spartan = spartan;
            this.stage = stage;
            this.metafile = metafile;
            this.repository = repository;
        }

        public SpartanService Spartan
        {
            get { return spartan; }
        }

        public SpartanHooks Hooks
        {
            get { return spartan.Hooks; }
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

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public RepositoryService Repository
        {
            get { return repository; }
        }

        public void Dispose()
        {
            metafile.Dispose();
            spartan.Dispose();
            sandbox.Dispose();
        }
    }
}
