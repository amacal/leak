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

        public DataGetToDataStore DataStore
        {
            get { return service.Dependencies.DataStore; }
        }

        public DataGetToGlue Glue
        {
            get { return service.Dependencies.Glue; }
        }

        public DataGetToDataMap DataMap
        {
            get { return service.Dependencies.DataMap; }
        }

        public void Dispose()
        {
        }
    }
}