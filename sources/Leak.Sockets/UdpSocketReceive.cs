using System;
using System.Net;

namespace Leak.Sockets
{
    public class UdpSocketReceive
    {
        private readonly SocketStatus status;
        private readonly int count;
        private readonly UdpSocket socket;
        private readonly SocketBuffer buffer;
        private readonly byte[] address;

        public UdpSocketReceive(SocketStatus status, int count, UdpSocket socket, SocketBuffer buffer, byte[] address)
        {
            this.status = status;
            this.count = count;
            this.socket = socket;
            this.buffer = buffer;
            this.address = address;
        }

        public SocketStatus Status
        {
            get { return status; }
        }

        public UdpSocket Socket
        {
            get { return socket; }
        }

        public SocketBuffer Buffer
        {
            get { return buffer; }
        }

        public int Count
        {
            get { return count; }
        }

        public IPEndPoint GetEndpoint()
        {
            int port = address[2] * 256 + address[3];
            byte[] data = new byte[4];

            Array.Copy(address, 4, data, 0, 4);
            return new IPEndPoint(new IPAddress(data), port);
        }
    }
}