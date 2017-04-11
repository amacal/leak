using System;
using System.Text;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeMessage : NetworkOutgoingMessage
    {
        public static readonly int MinSize = 1;
        public static readonly string ProtocolName = "BitTorrent protocol";

        private readonly PeerHash peer;
        private readonly FileHash hash;
        private readonly HandshakeOptions options;

        public HandshakeMessage(PeerHash peer, FileHash hash, HandshakeOptions options)
        {
            this.peer = peer;
            this.hash = hash;
            this.options = options;
        }

        public static int GetSize(NetworkIncomingMessage message)
        {
            return message[0] + 49;
        }

        public static PeerHash GetPeer(NetworkIncomingMessage message)
        {
            int length = message[0];
            byte[] data = message.ToBytes(29 + length, 20);

            return new PeerHash(data);
        }

        public static HandshakeOptions GetOptions(NetworkIncomingMessage message)
        {
            int length = message[0];
            uint value = Bytes.ReadUInt32(message.ToBytes(), 5 + length);

            return (HandshakeOptions)value;
        }

        public int Length
        {
            get { return 49 + ProtocolName.Length; }
        }

        public void ToBytes(DataBlock block)
        {
            int length = ProtocolName.Length;
            byte[] data = new byte[49 + length];

            data[0] = (byte)length;

            Bytes.Write((int)options, data, 5 + length);

            Array.Copy(Encoding.ASCII.GetBytes(ProtocolName), 0, data, 1, ProtocolName.Length);
            Array.Copy(hash.ToBytes(), 0, data, data.Length - 40, 20);
            Array.Copy(peer.ToBytes(), 0, data, data.Length - 20, 20);

            block.With((buffer, offset, count) =>
            {
                Array.Copy(data, 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}