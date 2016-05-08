using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public abstract class PeerChannelBase : PeerChannel
    {
        protected Func<PeerMessage, PeerMessage> Receiver;

        protected Func<PeerMessage, PeerMessage> Sender;

        protected Action<PeerBuffer, int> Remove;

        protected PeerChannelBase()
        {
            Receiver = x => x;
            Sender = x => x;
            Remove = (buffer, count) => buffer.Remove(count);
        }

        protected abstract Socket Socket { get; }

        protected abstract PeerCallback Callback { get; }

        protected abstract PeerBuffer Buffer { get; }

        protected void OnMessage(PeerMessage message)
        {
            try
            {
                message = Receiver.Invoke(message);

                if (message.Length == 0)
                {
                    Callback.OnTerminate(this);
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
                                Callback.OnKeepAlive(this);
                                break;

                            case 1:
                                Callback.OnUnchoke(this, new PeerUnchoke());
                                break;

                            case 5:
                                Callback.OnBitfield(this, new PeerBitfield(message));
                                break;

                            case 7:
                                Callback.OnPiece(this, new PeerPiece(message));
                                break;
                        }

                        Remove(Buffer, length + 4);
                        Buffer.ReceiveOrCallback(Socket, OnMessage);

                        return;
                    }
                }

                Buffer.Receive(Socket, OnMessage);
            }
            catch (SocketException)
            {
                Callback.OnTerminate(this);
            }
        }

        public override void Send(PeerMessageFactory data)
        {
            PeerMessage message = Sender.Invoke(data.GetMessage());
            byte[] bytes = message.ToBytes();

            Socket.Send(bytes);
        }
    }
}