using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHandler
    {
        private readonly PeerHash peer;
        private readonly DataBlockFactory factory;
        private readonly ConnectionLoopConnection connection;
        private readonly ConnectionLoopHooks hooks;

        public ConnectionLoopHandler(PeerHash peer, DataBlockFactory factory, ConnectionLoopConnection connection, ConnectionLoopHooks hooks)
        {
            this.peer = peer;
            this.factory = factory;
            this.hooks = hooks;
            this.connection = connection;
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
            if (message.Length == 4)
            {
                hooks.CallMessageReceived(peer, "keep-alive", message);
                message.Acknowledge(4);
            }
            else
            {
                switch (message[4])
                {
                    case 0:
                        hooks.CallMessageReceived(peer, "choke", message);
                        break;

                    case 1:
                        hooks.CallMessageReceived(peer, "unchoke", message);
                        break;

                    case 2:
                        hooks.CallMessageReceived(peer, "interested", message);
                        break;

                    case 4:
                        hooks.CallMessageReceived(peer, "have", message);
                        break;

                    case 5:
                        hooks.CallMessageReceived(peer, "bitfield", message);
                        break;

                    case 7:
                        hooks.CallMessageReceived(peer, "piece", message);
                        break;

                    case 20:
                        hooks.CallMessageReceived(peer, "extended", message);
                        break;
                }

                Acknowledge(message);
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