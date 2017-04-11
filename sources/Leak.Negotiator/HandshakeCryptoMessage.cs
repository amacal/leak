using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeCryptoMessage : NetworkOutgoingMessage
    {
        public static readonly int MinimumSize = 2;

        public static int GetSize(NetworkIncomingMessage message)
        {
            return 2 + message[0] * 256 + message[1];
        }

        public int Length
        {
            get { return 2; }
        }

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                Array.Copy(Bytes.Parse("0000"), 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}