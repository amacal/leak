using System;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataInstance : IDisposable
    {
        private readonly CoordinatorService service;

        public MetadataInstance(CoordinatorService service)
        {
            this.service = service;
        }

        public CoordinatorService Service
        {
            get { return service; }
        }

        public void Dispose()
        {
        }
    }
}