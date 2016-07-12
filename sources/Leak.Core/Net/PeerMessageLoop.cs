using Leak.Core.Network;
using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerMessageLoop : NetworkIncomingMessageHandler
    {
        private readonly PeerChannel channel;
        private readonly NetworkConnection connection;
        private readonly PeerMessageLoopConfiguration configuration;

        public PeerMessageLoop(PeerChannel channel, NetworkConnection connection, Action<PeerMessageLoopConfiguration> with)
        {
            this.channel = channel;
            this.connection = connection;

            configuration = new PeerMessageLoopConfiguration();
            with.Invoke(configuration);
        }

        public void OnMessage(NetworkIncomingMessage message)
        {
            try
            {
                if (message.Length == 0)
                {
                    configuration.Callback.OnTerminate(channel);
                    return;
                }

                if (message.Length >= 4)
                {
                    int length = message[3] + message[2] * 256 + message[1] * 256 * 256;
                    int id = 0;

                    if (message.Length >= length + 4)
                    {
                        if (message.Length > 4)
                        {
                            id = message[4];
                        }

                        switch (id)
                        {
                            case 0:
                                configuration.Callback.OnKeepAlive(channel);
                                break;

                            case 1:
                                configuration.Callback.OnUnchoke(channel, new PeerUnchoke());
                                break;

                            case 2:
                                configuration.Callback.OnInterested(channel, new PeerInterested());
                                break;

                            case 5:
                                configuration.Callback.OnBitfield(channel, new PeerBitfield(message));
                                break;

                            case 7:
                                configuration.Callback.OnPiece(channel, new PeerPiece(message));
                                break;

                            case 20:
                                configuration.Callback.OnExtended(channel, new PeerExtended(message));
                                break;

                            default:
                                break;
                        }

                        message.Acknowledge(length + 4);
                        connection.Receive(this);

                        return;
                    }
                }

                message.Continue(this);
            }
            catch (SocketException)
            {
                configuration.Callback.OnTerminate(channel);
            }
        }

        public void OnException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnected()
        {
            throw new NotImplementedException();
        }
    }
}