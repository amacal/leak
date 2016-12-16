using System;
using System.Threading.Tasks;
using Leak.Completion;
using Leak.Networking;
using Leak.Tasks;

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

            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(new NetworkPoolHooks());
            pool.Start();

            hooks = new PeerConnectorHooks();
        }

        public PeerConnectorHooks Hooks
        {
            get { return hooks; }
        }

        public ConnectorSession Start()
        {
            PeerConnectorConfiguration configuration = new PeerConnectorConfiguration();

            connector = new PeerConnector(pool, hooks, configuration);
            connector.Start(pipeline);

            return new ConnectorSession(pool, connector);
        }

        public void Dispose()
        {
            worker?.Dispose();
            pipeline?.Stop();
        }
    }
}
