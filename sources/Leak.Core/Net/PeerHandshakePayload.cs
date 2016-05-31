using System;

namespace Leak.Core.Net
{
    using Encoding = System.Text.Encoding;

    public class PeerHandshakePayload : PeerMessageFactory
    {
        public static readonly int MinSize = 1;

        private readonly string description;
        private readonly PeerHandshakeOptions options;

        private readonly byte[] hash;
        private readonly byte[] peer;

        public PeerHandshakePayload(byte[] hash, byte[] peer, PeerHandshakeOptions options)
        {
            this.description = "BitTorrent protocol";
            this.options = options;

            this.hash = hash;
            this.peer = peer;
        }

        public PeerHandshakePayload(PeerMessage message)
        {
            int length = message[0];

            this.description = Encoding.ASCII.GetString(message.ToBytes(1, length));
            this.options = (PeerHandshakeOptions)(message[8 + length] + (message[7 + length] << 8) + (message[6 + length] << 16));
            this.hash = message.ToBytes(9 + length, 20);
            this.peer = message.ToBytes(29 + length, 20);
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

        public PeerHandshakeOptions Options
        {
            get { return options; }
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