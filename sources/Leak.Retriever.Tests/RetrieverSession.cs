using System;
using Leak.Testing;

namespace Leak.Data.Get.Tests
{
    public class RetrieverSession : IDisposable
    {
        private readonly RetrieverService service;

        public RetrieverSession(RetrieverService service)
        {
            this.service = service;
        }

        public RetrieverService Service
        {
            get { return service; }
        }

        public PipelineSimulator Pipeline
        {
            get { return (PipelineSimulator)service.Dependencies.Pipeline; }
        }

        public RetrieverRepository Repository
        {
            get { return service.Dependencies.Repository; }
        }

        public RetrieverGlue Glue
        {
            get { return service.Dependencies.Glue; }
        }

        public RetrieverOmnibus Omnibus
        {
            get { return service.Dependencies.Omnibus; }
        }

        public void Dispose()
        {
        }
    }
}