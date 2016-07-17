using Leak.Core.Network;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHandler
    {
        private readonly ConnectionLoopConfiguration configuration;
        private readonly ConnectionLoopConnection connection;
        private readonly ConnectionLoopHandshake handshake;

        public ConnectionLoopHandler(ConnectionLoopConfiguration configuration, ConnectionLoopConnection connection, ConnectionLoopHandshake handshake)
        {
            this.configuration = configuration;
            this.handshake = handshake;
            this.connection = connection;
        }

        public void Execute()
        {
            configuration.Callback.OnConnected(new ConnectionLoopChannel(connection, handshake));
            connection.Receive(OnMessageHeader, 4);
        }

        private void OnMessageHeader(NetworkIncomingMessage message)
        {
            connection.Receive(OnMessageData, GetMessageSize(message) + 4);
        }

        private int GetMessageSize(NetworkIncomingMessage message)
        {
            return message[3] + message[2] * 256 + message[1] * 256 * 256;
        }

        private void OnMessageData(NetworkIncomingMessage message)
        {
            if (message.Length == 4)
            {
                Dispatch(message, configuration.Callback.OnKeepAlive);
                return;
            }

            switch (message[4])
            {
                case 1:
                    Dispatch(message, configuration.Callback.OnUnchoke);
                    break;

                case 2:
                    Dispatch(message, configuration.Callback.OnInterested);
                    break;

                case 5:
                    Dispatch(message, configuration.Callback.OnBitfield);
                    break;

                case 7:
                    Dispatch(message, configuration.Callback.OnPiece);
                    break;

                default:
                    message.Acknowledge(GetMessageSize(message) + 4);
                    connection.Receive(OnMessageHeader, 4);
                    break;
            }
        }

        private void Dispatch(NetworkIncomingMessage message, Action<ConnectionLoopChannel> callback)
        {
            message.Acknowledge(GetMessageSize(message) + 4);
            callback.Invoke(new ConnectionLoopChannel(connection, handshake));
            connection.Receive(OnMessageHeader, 4);
        }

        private void Dispatch(NetworkIncomingMessage message, Action<ConnectionLoopChannel, ConnectionLoopMessage> callback)
        {
            message.Acknowledge(GetMessageSize(message) + 4);
            callback.Invoke(new ConnectionLoopChannel(connection, handshake), new ConnectionLoopMessage(message));
            connection.Receive(OnMessageHeader, 4);
        }
    }
}