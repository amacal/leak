using System;
using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class InterestedMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 5; }
        }

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02 }, 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}