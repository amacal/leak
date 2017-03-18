using Leak.Completion;
using Leak.Networking;
using Leak.Tasks;
using System;

namespace Leak.Connector.Tests
{
    public class ConnectorFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly CompletionThread worker;
        private readonly NetworkPool pool;
        private PeerConnector connector;

        private readonly PeerConnectorHooks hooks;

        public ConnectorFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();

            worker = new CompletionThread();
            worker.Start();

            pool = new NetworkPoolBuilder().WithPipeline(pipeline).WithWorker(worker).Build();
            pool.Start();

            hooks = new PeerConnectorHooks();
        }

        public PeerConnectorHooks Hooks
        {
            get { return hooks; }
        }

        public ConnectorSession Start()
        {
            connector =
                new PeerConnectorBuilder()
                    .WithPipeline(pipeline)
                    .WithNetwork(pool)
                    .Build(hooks);

            connector.Start();

            return new ConnectorSession(pool, connector);
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}