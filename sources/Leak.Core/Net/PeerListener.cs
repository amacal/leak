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

        public void Start(PeerHandshake handshake)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            socket.Listen(1000);
            socket.BeginAccept(OnAccepted, handshake);
        }

        private void OnAccepted(IAsyncResult result)
        {
            try
            {
                PeerHandshake handshake = result.AsyncState as PeerHandshake;
                Socket endpoint = socket.EndAccept(result);

                PeerBuffer buffer = new PeerBuffer(40000);
                Channel channel = new Channel(endpoint, buffer, callback);

                buffer.Receive(endpoint, message =>
                {
                    if (message.Length == 0)
                        return;

                    if (message.Length >= message[0] + 49)
                    {
                        channel.Send(handshake);
                        callback.OnHandshake(channel, new PeerHandshake(message));

                        buffer.Remove(message[0] + 49);
                        channel.Start();
                    }
                });
            }
            catch (SocketException)
            {
            }
        }

        private class Channel : PeerChannelBase
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

            public void Start()
            {
                buffer.ReceiveOrCallback(socket, OnMessage);
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