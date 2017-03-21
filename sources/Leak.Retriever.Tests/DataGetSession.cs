using System;
using Leak.Testing;

namespace Leak.Data.Get.Tests
{
    public class DataGetSession : IDisposable
    {
        private readonly DataGetService service;

        public DataGetSession(DataGetService service)
        {
            this.service = service;
        }

        public DataGetService Service
        {
            get { return service; }
        }

        public PipelineSimulator Pipeline
        {
            get { return (PipelineSimulator)service.Dependencies.Pipeline; }
        }

        public DataGetToDataStore Repository
        {
            get { return service.Dependencies.Repository; }
        }

        public DataGetToGlue Glue
        {
            get { return service.Dependencies.Glue; }
        }

        public DataGetToDataMap Omnibus
        {
            get { return service.Dependencies.Omnibus; }
        }

        public void Dispose()
        {
        }
    }
}