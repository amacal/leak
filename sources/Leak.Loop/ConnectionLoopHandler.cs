using Leak.Common;

namespace Leak.Loop
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
                        hooks.CallMessageReceived(peer, "choke", message.Restrict());
                        break;

                    case 1:
                        hooks.CallMessageReceived(peer, "unchoke", message.Restrict());
                        break;

                    case 2:
                        hooks.CallMessageReceived(peer, "interested", message.Restrict());
                        break;

                    case 4:
                        hooks.CallMessageReceived(peer, "have", message.Restrict());
                        break;

                    case 5:
                        hooks.CallMessageReceived(peer, "bitfield", message.Restrict());
                        break;

                    case 6:
                        hooks.CallMessageReceived(peer, "request", message.Restrict());
                        break;

                    case 7:
                        hooks.CallMessageReceived(peer, "piece", message.Restrict());
                        break;

                    case 20:
                        hooks.CallMessageReceived(peer, "extended", message.Restrict());
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