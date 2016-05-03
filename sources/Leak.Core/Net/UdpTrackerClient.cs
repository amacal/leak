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

        public override TrackerResonse Announce(PeerHandshake handshake)
        {
            int received;

            byte[] connect = new byte[16];
            byte[] announce = new byte[98];
            byte[] response = new byte[4000];
            byte[] transaction = Bytes.Random(4);

            Array.Copy(Bytes.Parse("0000041727101980"), connect, 8);

            Array.Copy(Bytes.Parse("00000001"), 0, announce, 8, 4);
            Array.Copy(transaction, 0, announce, 12, 4);
            Array.Copy(handshake.Hash, 0, announce, 16, 20);
            Array.Copy(handshake.Peer, 0, announce, 36, 20);

            Array.Copy(Bytes.Parse("0000000000010000"), 0, announce, 64, 8);
            Array.Copy(Bytes.Parse("00000001"), 0, announce, 80, 4);
            Array.Copy(Bytes.Parse("ffff"), 0, announce, 92, 2);
            Array.Copy(Bytes.Parse("1f90"), 0, announce, 96, 2);

            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(host, port);

                socket.Send(connect);
                received = socket.Receive(response);

                Array.Copy(response, 8, announce, 0, 8);

                socket.Send(announce);
                received = socket.Receive(response);

                return new UdpTrackerResponse(response, received);
            }
        }
    }
}