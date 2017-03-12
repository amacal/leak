using System;
using F2F.Sandbox;
using Leak.Common;

namespace Leak.Data.Store.Tests
{
    public class RepositorySession : IDisposable
    {
        private readonly Metainfo metainfo;
        private readonly RepositoryService service;
        private readonly IFileSandbox sandbox;
        private readonly RepositoryData data;

        public RepositorySession(Metainfo metainfo, RepositoryService service, IFileSandbox sandbox, RepositoryData data)
        {
            this.metainfo = metainfo;
            this.service = service;
            this.sandbox = sandbox;
            this.data = data;
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public RepositoryHooks Hooks
        {
            get { return service.Hooks; }
        }

        public IFileSandbox Sandbox
        {
            get { return sandbox; }
        }

        public FileHash Hash
        {
            get { return service.Hash; }
        }

        public RepositoryData Data
        {
            get { return data; }
        }

        public RepositoryService Service
        {
            get { return service; }
        }

        public void Dispose()
        {
            service.Dispose();
            sandbox.Dispose();
        }
    }
}