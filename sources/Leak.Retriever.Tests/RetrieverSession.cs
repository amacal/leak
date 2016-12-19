using System;
using Leak.Common;

namespace Leak.Retriever.Tests
{
    public class RetrieverSession : IDisposable
    {
        private readonly RetrieverService service;
        private readonly RetrieverData data;
        private readonly FileHash hash;
        private readonly RetrieverHooks hooks;

        public RetrieverSession(RetrieverService service, RetrieverData data, FileHash hash, RetrieverHooks hooks)
        {
            this.service = service;
            this.data = data;
            this.hash = hash;
            this.hooks = hooks;
        }

        public RetrieverHooks Hooks
        {
            get { return hooks; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public RetrieverService Service
        {
            get { return service; }
        }

        public RetrieverData Data
        {
            get { return data; }
        }

        public void Dispose()
        {
        }
    }
}
