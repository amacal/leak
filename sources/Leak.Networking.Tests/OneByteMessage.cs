using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Networking.Tests
{
    public class OneByteMessage : NetworkOutgoingMessage
    {
        public int Length
        {
            get { return 1; }
        }

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                Array.Copy(new byte[] { 0x00 }, 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}