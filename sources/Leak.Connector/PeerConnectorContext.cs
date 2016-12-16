using Leak.Networking;
using Leak.Tasks;

namespace Leak.Connector
{
    public class PeerConnectorContext
    {
        private readonly NetworkPool pool;
        private readonly PeerConnectorHooks hooks;
        private readonly PeerConnectorConfiguration configuration;
        private readonly LeakQueue<PeerConnectorContext> queue;

        public PeerConnectorContext(NetworkPool pool, PeerConnectorHooks hooks, PeerConnectorConfiguration configuration)
        {
            this.pool = pool;
            this.hooks = hooks;
            this.configuration = configuration;

            queue = new LeakQueue<PeerConnectorContext>(this);
        }

        public PeerConnectorConfiguration Configuration
        {
            get { return configuration; }
        }

        public NetworkPool Pool
        {
            get { return pool; }
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