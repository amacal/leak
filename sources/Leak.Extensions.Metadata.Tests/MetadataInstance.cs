using System;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataInstance : IDisposable
    {
        private readonly GlueService service;

        public MetadataInstance(GlueService service)
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