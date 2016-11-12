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
            configuration.Callback.OnAttached(new ConnectionLoopChannel(configuration, connection, handshake));
            connection.Receive(OnMessageHeader, 4);
        }

        private void OnMessageHeader(NetworkIncomingMessage message)
        {
            connection.Receive(OnMessageData, ConnectionLoopMessage.GetMessageSize(message) + 4);
        }

        private void OnMessageData(NetworkIncomingMessage message)
        {
            if (message.Length == 4)
            {
                Notify("keep-alive");
                Dispatch(message, configuration.Callback.OnKeepAlive);

                return;
            }

            switch (message[4])
            {
                case 0:
                    Dispatch(message, configuration.Callback.OnChoke);
                    break;

                case 1:
                    Dispatch(message, configuration.Callback.OnUnchoke);
                    break;

                case 2:
                    Dispatch(message, configuration.Callback.OnInterested);
                    break;

                case 4:
                    Dispatch(message, configuration.Callback.OnHave);
                    break;

                case 5:
                    Dispatch(message, configuration.Callback.OnBitfield);
                    break;

                case 7:
                    Dispatch(message, configuration.Callback.OnPiece);
                    break;

                case 20:
                    Dispatch(message, configuration.Callback.OnExtended);
                    break;

                default:
                    Ignore(message);
                    break;
            }
        }

        private void Ignore(NetworkIncomingMessage message)
        {
            message.Acknowledge(ConnectionLoopMessage.GetMessageSize(message) + 4);
            connection.Receive(OnMessageHeader, 4);
        }

        private void Notify(string type)
        {
        }

        private void Dispatch(NetworkIncomingMessage message, Action<ConnectionLoopChannel> callback)
        {
            message.Acknowledge(ConnectionLoopMessage.GetMessageSize(message) + 4);
            callback.Invoke(new ConnectionLoopChannel(configuration, connection, handshake));
            connection.Receive(OnMessageHeader, 4);
        }

        private void Dispatch(NetworkIncomingMessage message, Action<ConnectionLoopChannel, ConnectionLoopMessage> callback)
        {
            message.Acknowledge(ConnectionLoopMessage.GetMessageSize(message) + 4);
            callback.Invoke(new ConnectionLoopChannel(configuration, connection, handshake), new ConnectionLoopMessage(message));
            connection.Receive(OnMessageHeader, 4);
        }
    }
}