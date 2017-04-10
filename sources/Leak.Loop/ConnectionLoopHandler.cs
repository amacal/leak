using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Loop
{
    public class ConnectionLoopHandler
    {
        private readonly PeerHash peer;
        private readonly ConnectionLoopConnection connection;

        private readonly ConnectionLoopConfiguration configuration;
        private readonly ConnectionLoopHooks hooks;

        public ConnectionLoopHandler(PeerHash peer, ConnectionLoopConnection connection, ConnectionLoopConfiguration configuration, ConnectionLoopHooks hooks)
        {
            this.peer = peer;
            this.hooks = hooks;
            this.connection = connection;
            this.configuration = configuration;
        }

        public void Execute()
        {
            connection.Receive(OnMessageHeader, 4);
        }

        private void OnMessageHeader(NetworkIncomingMessage message)
        {
            connection.Receive(OnMessageData, message.GetSize() + 4);
        }

        private void OnMessageData(NetworkIncomingMessage message)
        {
            if (message.Length >= 4)
            {
                if (message[0] == 0 && message[1] == 0 && message[2] == 0 && message[3] == 0)
                {
                    hooks.CallMessageReceived(peer, "keep-alive", message.Restrict());
                    message.Acknowledge(4);

                    Next();
                    return;
                }
            }

            if (message.Length > 4)
            {
                string name;
                int id = message[4];

                if (configuration.Messages.TryGetValue(id, out name))
                {
                    hooks.CallMessageReceived(peer, name, message.Restrict());
                }

                Acknowledge(message);
                Next();

                return;
            }

            Next();
        }

        private void Acknowledge(NetworkIncomingMessage message)
        {
            message.Acknowledge(message.GetSize() + 4);
        }

        private void Next()
        {
            connection.Receive(OnMessageHeader, 4);
        }
    }
}