using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerClient : PeerChannelBase, PeerNegotiatorAware
    {
        private readonly Socket socket;
        private readonly PeerCallback callback;
        private readonly PeerBuffer buffer;

        private readonly string host;
        private readonly int port;

        public PeerClient(PeerCallback callback, string host, int port)
        {
            this.socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.buffer = new PeerBuffer(40000);
            this.callback = callback;

            this.host = host;
            this.port = port;
        }

        public override PeerDescription Description
        {
            get { return new ClientDescription(host, port); }
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

        public void Start(PeerNegotiator negotiator)
        {
            socket.BeginConnect(host, port, OnConnected, negotiator);
        }

        private void OnConnected(IAsyncResult result)
        {
            PeerNegotiator negotiator = (PeerNegotiator)result.AsyncState;

            try
            {
                socket.EndConnect(result);
                negotiator.Active(this);
            }
            catch (SocketException)
            {
                callback.OnTerminate(this);
            }
        }

        public void Stop()
        {
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

        private class ClientDescription : PeerDescription
        {
            private readonly string host;
            private readonly int port;

            public ClientDescription(string host, int port)
            {
                this.host = host;
                this.port = port;
            }

            public override string Host
            {
                get { return host; }
            }

            public override string ToString()
            {
                return $"> {host}";
            }
        }
    }
}