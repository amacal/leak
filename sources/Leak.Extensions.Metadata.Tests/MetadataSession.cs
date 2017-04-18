using System;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataSession : IDisposable
    {
        private readonly CoordinatorService coordinator;
        private readonly MetadataPlugin plugin;

        public MetadataSession(CoordinatorService coordinator, MetadataPlugin plugin)
        {
            this.coordinator = coordinator;
            this.plugin = plugin;
        }

        public CoordinatorService Coordinator
        {
            get { return coordinator; }
        }

        public MetadataPlugin Plugin
        {
            get { return plugin; }
        }

        public void Dispose()
        {
        }
    }
}