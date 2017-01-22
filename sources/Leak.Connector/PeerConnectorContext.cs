using Leak.Tasks;

namespace Leak.Connector
{
    public class PeerConnectorContext
    {
        private readonly PeerConnectorDependencies dependencies;
        private readonly PeerConnectorHooks hooks;
        private readonly PeerConnectorConfiguration configuration;
        private readonly LeakQueue<PeerConnectorContext> queue;

        public PeerConnectorContext(PeerConnectorDependencies dependencies, PeerConnectorHooks hooks, PeerConnectorConfiguration configuration)
        {
            this.dependencies = dependencies;
            this.hooks = hooks;
            this.configuration = configuration;

            queue = new LeakQueue<PeerConnectorContext>(this);
        }

        public PeerConnectorConfiguration Configuration
        {
            get { return configuration; }
        }

        public PeerConnectorDependencies Dependencies
        {
            get { return dependencies; }
        }

        public LeakQueue<PeerConnectorContext> Queue
        {
            get { return queue; }
        }

        public PeerConnectorHooks Hooks
        {
            get { return hooks; }
        }
    }
}