using System;
using Leak.Common;
using Leak.Tasks;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;

        public CoordinatorFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();
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
            pipeline?.Stop();
        }
    }
}