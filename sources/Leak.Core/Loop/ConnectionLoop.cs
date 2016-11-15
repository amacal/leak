using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Loop
{
    public class ConnectionLoop
    {
        private readonly DataBlockFactory factory;
        private readonly ConnectionLoopHooks hooks;
        private readonly ConnectionLoopConfiguration configuration;

        public ConnectionLoop(DataBlockFactory factory, ConnectionLoopHooks hooks, ConnectionLoopConfiguration configuration)
        {
            this.factory = factory;
            this.hooks = hooks;
            this.configuration = configuration;
        }

        public void StartProcessing(PeerHash peer, NetworkConnection connection)
        {
            ConnectionLoopConnection wrapped = new ConnectionLoopConnection(connection);
            ConnectionLoopHandler handler = new ConnectionLoopHandler(peer, factory, wrapped, hooks);

            handler.Execute();
        }
    }
}