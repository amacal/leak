using System;
using Leak.Common;
using Leak.Testing;

namespace Leak.Data.Share.Tests
{
    public class DataShareSession : IDisposable
    {
        private readonly DataShareService service;

        public DataShareSession(DataShareService service)
        {
            this.service = service;
        }

        public DataShareService Service
        {
            get { return service; }
        }

        public PipelineSimulator Pipeline
        {
            get { return (PipelineSimulator)service.Dependencies.Pipeline; }
        }

        public DataShareToDataStore DataStore
        {
            get { return service.Dependencies.DataStore; }
        }

        public DataShareToDataMap DataMap
        {
            get { return service.Dependencies.DataMap; }
        }

        public DataShareToGlue Glue
        {
            get { return service.Dependencies.Glue; }
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}