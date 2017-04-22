using System;
using Leak.Common;
using Leak.Testing;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorFixture : IDisposable
    {
        private readonly PipelineSimulator pipeline;

        public CoordinatorFixture()
        {
            pipeline = new PipelineSimulator();
        }

        public CoordinatorSession Start()
        {
            CoordinatorService coordinator =
                new CoordinatorBuilder()
                    .WithHash(FileHash.Random())
                    .WithPipeline(pipeline)
                    .Build();

            return new CoordinatorSession(coordinator);
        }

        public void Dispose()
        {
        }
    }
}