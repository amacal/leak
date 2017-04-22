using System;
using Leak.Common;
using Leak.Testing;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorSession : IDisposable
    {
        private readonly CoordinatorService coordinator;

        public CoordinatorSession(CoordinatorService coordinator)
        {
            this.coordinator = coordinator;
        }

        public CoordinatorService Coordinator
        {
            get { return coordinator; }
        }

        public FileHash Hash
        {
            get { return coordinator.Hash; }
        }

        public PipelineSimulator Pipeline
        {
            get { return (PipelineSimulator)coordinator.Dependencies.Pipeline; }
        }

        public void Dispose()
        {
        }
    }
}