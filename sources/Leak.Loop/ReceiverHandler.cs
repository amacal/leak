using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Receiver
{
    public class ReceiverHandler
    {
        private readonly PeerHash peer;
        private readonly ReceiverConnection connection;

        private readonly ReceiverConfiguration configuration;
        private readonly ReceiverHooks hooks;

        public ReceiverHandler(PeerHash peer, ReceiverConnection connection, ReceiverConfiguration configuration, ReceiverHooks hooks)
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
                    hooks.CallKeepAliveReceived(peer);
                    message.Acknowledge(4);

                    Next();
                    return;
                }
            }

            if (message.Length > 4)
            {
                byte identifier = message[4];
                string name = configuration.Definition?.GetName(identifier);

                if (String.IsNullOrWhiteSpace(name) == false)
                {
                    hooks.CallMessageReceived(peer, name, message.Restricted());
                }
                else
                {
                    hooks.CallMessageIgnored(peer, identifier, message.Restricted());
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