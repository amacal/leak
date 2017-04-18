using Leak.Common;
using Leak.Tasks;
using System;
using System.Net;
using System.Threading.Tasks;
using Leak.Networking.Core;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;

        public PeersFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();
        }

        public PeersSession Start()
        {
            PeersHooks hooks = new PeersHooks();
            PeersPlugin plugin = new PeersPlugin(hooks);

            CoordinatorService coordinator =
                new CoordinatorBuilder()
                    .WithHash(FileHash.Random())
                    .WithPipeline(pipeline)
                    .WithPlugin(plugin)
                    .Build();

            return new PeersSession(coordinator, plugin);
        }

        public void Dispose()
        {
            pipeline?.Stop();
        }
    }
}