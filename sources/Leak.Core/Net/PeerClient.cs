using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerClient : PeerChannelBase
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

        public void Start(PeerHandshake handshake)
        {
            socket.BeginConnect(host, port, OnConnected, handshake);
        }

        private void OnConnected(IAsyncResult result)
        {
            try
            {
                socket.EndConnect(result);

                Send(result.AsyncState as PeerHandshake);
                buffer.Receive(socket, OnHandshake);
            }
            catch (SocketException)
            {
                callback.OnTerminate(this);
            }
        }

        public void Stop()
        {
        }

        private void OnHandshake(PeerMessage message)
        {
            if (message.Length == 0)
            {
                callback.OnTerminate(this);
                return;
            }

            if (message.Length >= message[0] + 49)
            {
                callback.OnHandshake(this, new PeerHandshake(message));
                buffer.Remove(message[0] + 49);
            }

            buffer.ReceiveOrCallback(socket, OnMessage);
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