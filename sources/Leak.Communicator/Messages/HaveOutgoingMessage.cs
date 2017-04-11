using System;
using Leak.Networking.Core;

namespace Leak.Peer.Communicator.Messages
{
    public class HaveOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly int piece;

        public HaveOutgoingMessage(int piece)
        {
            this.piece = piece;
        }

        public int Length
        {
            get { return 9; }
        }

        public void ToBytes(DataBlock block)
        {
            byte lowest = (byte)(piece % 256);
            byte highest = (byte)(piece / 256);

            block.With((buffer, offset, count) =>
            {
                Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x05, 0x04, 0x00, 0x00, highest, lowest }, 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}