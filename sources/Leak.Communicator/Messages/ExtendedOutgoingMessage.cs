using System;
using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class ExtendedOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly Extended extended;

        public ExtendedOutgoingMessage(Extended extended)
        {
            this.extended = extended;
        }

        public int Length
        {
            get { return 6 + extended.Data.Length; }
        }

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                buffer[offset + 4] = 0x14;
                buffer[offset + 5] = extended.Id;

                Bytes.Write(extended.Data.Length + 2, buffer, offset);
                Array.Copy(extended.Data, 0, buffer, offset + 6, extended.Data.Length);
            });
        }

        public void Release()
        {
        }
    }
}