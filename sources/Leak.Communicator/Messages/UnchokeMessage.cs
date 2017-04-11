using System;
using Leak.Networking.Core;

namespace Leak.Peer.Communicator.Messages
{
    public class UnchokeMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 5; }
        }

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01 }, 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}