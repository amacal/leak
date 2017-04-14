using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Sender
{
    public class SenderService
    {
        private readonly PeerHash peer;
        private readonly NetworkConnection connection;
        private readonly SenderHooks hooks;
        private readonly SenderConfiguration configuration;

        public SenderService(PeerHash peer, NetworkConnection connection, SenderHooks hooks, SenderConfiguration configuration)
        {
            this.peer = peer;
            this.connection = connection;
            this.hooks = hooks;
            this.configuration = configuration;
        }

        public void SendKeepAlive()
        {
            connection.Send(new SenderKeepAliveMessage());
            hooks.CallKeepAliveSent(peer);
        }

        public void Send(SenderOutgoingMessage message)
        {
            string type = message.Type;
            byte? identifier = configuration.Definition?.GetIdentifier(type);

            if (identifier != null)
            {
                connection.Send(message.Apply(identifier.Value));
                hooks.CallMessageSent(peer, type, message);
            }
            else
            {
                hooks.CallMessageIgnored(peer, type, message);
                message.Release();
            }
        }
    }
}