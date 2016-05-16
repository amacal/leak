using System;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class UdpTrackerClient : TrackerClient
    {
        private readonly string host;
        private readonly int port;

        public UdpTrackerClient(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public override TrackerResonse Announce(PeerAnnounce announce)
        {
            int received;

            byte[] connect = new byte[16];
            byte[] request = new byte[98];
            byte[] response = new byte[4000];
            byte[] transaction = Bytes.Random(4);

            Array.Copy(Bytes.Parse("0000041727101980"), connect, 8);

            Array.Copy(Bytes.Parse("00000001"), 0, request, 8, 4);
            Array.Copy(transaction, 0, request, 12, 4);
            Array.Copy(announce.Handshake.Hash, 0, request, 16, 20);
            Array.Copy(announce.Handshake.Peer, 0, request, 36, 20);

            Array.Copy(Bytes.Parse("0000000000010000"), 0, request, 64, 8);
            Array.Copy(Bytes.Parse("00000001"), 0, request, 80, 4);
            Array.Copy(Bytes.Parse("ffff"), 0, request, 92, 2);
            Array.Copy(Bytes.Parse("1f91"), 0, request, 96, 2);

            if (announce.Address != null)
            {
                Array.Copy(announce.Address, 0, request, 84, 4);
            }

            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                socket.SendTimeout = 15000;
                socket.ReceiveTimeout = 15000;
                socket.Connect(host, port);

                socket.Send(connect);
                received = socket.Receive(response);

                Array.Copy(response, 8, request, 0, 8);

                socket.Send(request);
                received = socket.Receive(response);

                return new UdpTrackerResponse(response, received);
            }
        }
    }
}