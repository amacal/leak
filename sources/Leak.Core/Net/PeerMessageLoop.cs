using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerMessageLoop
    {
        private readonly PeerChannel channel;
        private readonly PeerConnection connection;
        private readonly PeerMessageLoopConfiguration configuration;

        public PeerMessageLoop(PeerChannel channel, PeerConnection connection, Action<PeerMessageLoopConfiguration> with)
        {
            this.channel = channel;
            this.connection = connection;

            configuration = new PeerMessageLoopConfiguration();
            with.Invoke(configuration);
        }

        public void Process(PeerMessage message)
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

                            case 5:
                                configuration.Callback.OnBitfield(channel, new PeerBitfield(message));
                                break;

                            case 7:
                                configuration.Callback.OnPiece(channel, new PeerPiece(message));
                                break;
                        }

                        connection.Remove(length + 4);
                        connection.ReceiveOrCallback(Process);

                        return;
                    }
                }

                connection.Receive(Process);
            }
            catch (SocketException)
            {
                configuration.Callback.OnTerminate(channel);
            }
        }
    }
}