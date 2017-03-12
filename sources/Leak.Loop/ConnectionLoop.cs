using Leak.Common;

namespace Leak.Loop
{
    public class ConnectionLoop
    {
        private readonly ConnectionLoopHooks hooks;
        private readonly ConnectionLoopConfiguration configuration;

        public ConnectionLoop(ConnectionLoopHooks hooks, ConnectionLoopConfiguration configuration)
        {
            this.hooks = hooks;
            this.configuration = configuration;
        }

        public void StartProcessing(PeerHash peer, NetworkConnection connection)
        {
            ConnectionLoopConnection wrapped = new ConnectionLoopConnection(connection);
            ConnectionLoopHandler handler = new ConnectionLoopHandler(peer, wrapped, hooks);

            handler.Execute();
        }
    }
}