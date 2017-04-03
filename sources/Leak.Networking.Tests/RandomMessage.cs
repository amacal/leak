using System;
using Leak.Common;

namespace Leak.Networking.Tests
{
    public class RandomMessage : NetworkOutgoingMessage
    {
        private readonly int length;

        public RandomMessage(int length)
        {
            this.length = length;
        }

        public int Length
        {
            get { return length; }
        }

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                Array.Copy(Bytes.Random(length), 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}