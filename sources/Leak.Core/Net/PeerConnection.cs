using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerConnection
    {
        private readonly Socket socket;
        private readonly PeerBuffer buffer;
        private readonly PeerConnectionConfiguration configuration;

        public PeerConnection(Socket socket)
        {
            this.socket = socket;
            this.configuration = new PeerConnectionConfiguration
            {
                Encrypt = data => data,
                Decrypt = data => data,
            };

            this.buffer = new PeerBuffer(socket, with =>
            {
                with.Size = 40000;
                with.Decrypt = configuration.Decrypt;
            });
        }

        public PeerConnection(PeerConnection connection, Action<PeerConnectionConfiguration> configurer)
        {
            socket = connection.socket;
            configuration = new PeerConnectionConfiguration
            {
                Encrypt = connection.configuration.Encrypt,
                Decrypt = connection.configuration.Decrypt
            };

            configurer.Invoke(configuration);
            buffer = new PeerBuffer(connection.buffer, with =>
            {
                with.Size = 40000;
                with.Decrypt = configuration.Decrypt;
            });
        }

        public string Remote
        {
            get { return socket.RemoteEndPoint.ToString(); }
        }

        public void Receive(Action<PeerMessage> callback)
        {
            buffer.Receive(callback);
        }

        public void ReceiveOrCallback(Action<PeerMessage> callback)
        {
            buffer.ReceiveOrCallback(callback);
        }

        public void Send(PeerMessage message)
        {
            byte[] decrypted = message.ToBytes();
            byte[] encrypted = configuration.Encrypt.Invoke(decrypted);

            socket.Send(encrypted);
        }

        public void Remove(int bytes)
        {
            buffer.Remove(bytes);
        }
    }
}