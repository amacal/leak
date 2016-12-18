using System;
using F2F.Sandbox;
using Leak.Common;
using Leak.Core.Repository;

namespace Leak.Repository.Tests
{
    public class RepositorySession : IDisposable
    {
        private readonly RepositoryService service;
        private readonly RepositoryHooks hooks;
        private readonly IFileSandbox sandbox;
        private readonly FileHash hash;
        private readonly RepositoryData data;

        public RepositorySession(RepositoryService service, RepositoryHooks hooks, IFileSandbox sandbox, FileHash hash, RepositoryData data)
        {
            this.service = service;
            this.hooks = hooks;
            this.sandbox = sandbox;
            this.hash = hash;
            this.data = data;
        }

        public RepositoryHooks Hooks
        {
            get { return hooks; }
        }

        public IFileSandbox Sandbox
        {
            get { return sandbox; }
        }

        public FileHash Hash
        {
            get { return hash; }
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