using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Communicator
{
    public class CommunicatorService
    {
        private readonly PeerHash peer;
        private readonly NetworkConnection connection;
        private readonly CommunicatorHooks hooks;
        private readonly CommunicatorConfiguration configuration;

        public CommunicatorService(PeerHash peer, NetworkConnection connection, CommunicatorHooks hooks, CommunicatorConfiguration configuration)
        {
            this.peer = peer;
            this.connection = connection;
            this.hooks = hooks;
            this.configuration = configuration;
        }

        public void SendKeepAlive()
        {
            connection.Send(new KeepAliveMessage());
        }
    }
}