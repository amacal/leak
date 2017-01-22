using System;

namespace Leak.Glue.Tests
{
    public class GlueInstance : IDisposable
    {
        private readonly GlueService service;

        public GlueInstance(GlueService service)
        {
            this.service = service;
        }

        public GlueService Service
        {
            get { return service; }
        }

        public void Dispose()
        {
        }
    }
}