using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Communicator.Messages
{
    public class BitfieldOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly byte[] data;

        public BitfieldOutgoingMessage(Bitfield bitfield)
        {
            this.data = new byte[(bitfield.Length - 1) / 8 + 1];

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (bitfield[i])
                {
                    data[i / 8] += (byte)(1 << (byte)(7 - i % 8));
                }
            }
        }

        public int Length
        {
            get { return data.Length + 5; }
        }

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                buffer[offset + 4] = 0x05;

                Bytes.Write(data.Length + 1, buffer, offset);
                Array.Copy(data, 0, buffer, offset + 5, data.Length);
            });
        }

        public void Release()
        {
        }
    }
}