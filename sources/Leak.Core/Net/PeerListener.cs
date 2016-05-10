using System;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerListener
    {
        private readonly Socket socket;
        private readonly PeerCallback callback;

        public PeerListener(PeerCallback callback)
        {
            this.socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.callback = callback;
        }

        public void Start(PeerNegotiator negotiator)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            socket.Listen(1000);
            socket.BeginAccept(OnAccepted, negotiator);
        }

        private void OnAccepted(IAsyncResult result)
        {
            try
            {
                PeerNegotiator negotiator = result.AsyncState as PeerNegotiator;
                Socket endpoint = socket.EndAccept(result);

                PeerBuffer buffer = new PeerBuffer(40000);
                Channel channel = new Channel(endpoint, buffer, callback);

                negotiator.Passive(channel);
            }
            catch (SocketException)
            {
            }
        }

        private class Channel : PeerChannelBase, PeerNegotiatorAware
        {
            private readonly Socket socket;
            private readonly PeerCallback callback;
            private readonly PeerBuffer buffer;

            public Channel(Socket socket, PeerBuffer buffer, PeerCallback callback)
            {
                this.socket = socket;
                this.buffer = buffer;
                this.callback = callback;
            }

            public override PeerDescription Description
            {
                get { return new ClientDescription(socket.RemoteEndPoint); }
            }

            protected override Socket Socket
            {
                get { return socket; }
            }

            protected override PeerBuffer Buffer
            {
                get { return buffer; }
            }

            protected override PeerCallback Callback
            {
                get { return callback; }
            }

            void PeerNegotiatorAware.Receive(Func<PeerMessage, bool> predicate, Action<PeerMessage> callback)
            {
                Action<PeerMessage> onMessage = null;

                onMessage = message =>
                {
                    if (predicate.Invoke(message))
                    {
                        callback.Invoke(message);
                    }
                    else
                    {
                        buffer.Receive(socket, onMessage);
                    }
                };

                buffer.ReceiveOrCallback(socket, onMessage);
            }

            void PeerNegotiatorAware.Send(PeerMessageFactory data)
            {
                PeerMessage message = data.GetMessage();
                byte[] bytes = message.ToBytes();

                Socket.Send(bytes);
            }

            void PeerNegotiatorAware.Remove(int length)
            {
                buffer.Remove(length);
            }

            void PeerNegotiatorAware.Continue(PeerHandshake handshake, Func<PeerMessage, PeerMessage> encrypt, Func<PeerMessage, PeerMessage> decrypt, Action<PeerBuffer, int> remove)
            {
                this.Receiver = decrypt;
                this.Sender = encrypt;
                this.Remove = remove;

                callback.OnHandshake(this, handshake);
                buffer.ReceiveOrCallback(socket, OnMessage);
            }

            void PeerNegotiatorAware.Terminate()
            {
                Callback.OnTerminate(this);
            }
        }

        private class ClientDescription : PeerDescription
        {
            private readonly EndPoint endpoint;

            public ClientDescription(EndPoint endpoint)
            {
                this.endpoint = endpoint;
            }

            public override string Host
            {
                get { return endpoint.ToString(); }
            }

            public override string ToString()
            {
                return $"< {endpoint}";
            }
        }
    }
}