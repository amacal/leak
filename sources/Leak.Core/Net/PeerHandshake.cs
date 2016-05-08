using System;

namespace Leak.Core.Net
{
    using Encoding = System.Text.Encoding;

    public class PeerHandshake : PeerMessageFactory
    {
        public static readonly int MinSize = 1;

        private readonly string description;
        private readonly byte[] hash;
        private readonly byte[] peer;

        public PeerHandshake(byte[] hash, byte[] peer)
        {
            this.description = "BitTorrent protocol";
            this.hash = hash;
            this.peer = peer;
        }

        public PeerHandshake(PeerMessage message)
        {
            int length = message[0];

            this.description = Encoding.ASCII.GetString(message.ToBytes(1, length));
            this.hash = message.ToBytes(1 + length, 20);
            this.peer = message.ToBytes(21 + length, 20);

            for (int i = 0; i < 20; i++)
            {
                this.peer[i] = (byte)((this.peer[i] + 1) % 256);
            }
        }

        public static int GetSize(PeerMessage message)
        {
            return message[0] + 49;
        }

        public byte[] Hash
        {
            get { return hash; }
        }

        public byte[] Peer
        {
            get { return peer; }
        }

        public override PeerMessage GetMessage()
        {
            byte[] data = new byte[49 + description.Length];

            data[0] = (byte)description.Length;

            Array.Copy(Encoding.ASCII.GetBytes(description), 0, data, 1, description.Length);
            Array.Copy(hash, 0, data, data.Length - 40, 20);
            Array.Copy(peer, 0, data, data.Length - 20, 20);

            return new PeerMessage(data);
        }
    }
}