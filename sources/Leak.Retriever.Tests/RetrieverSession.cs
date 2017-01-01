using System;
using Leak.Common;

namespace Leak.Retriever.Tests
{
    public class RetrieverSession : IDisposable
    {
        private readonly RetrieverService service;
        private readonly RetrieverData data;

        public RetrieverSession(RetrieverService service, RetrieverData data)
        {
            this.service = service;
            this.data = data;
        }

        public RetrieverHooks Hooks
        {
            get { return service.Hooks; }
        }

        public FileHash Hash
        {
            get { return service.Hash; }
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
