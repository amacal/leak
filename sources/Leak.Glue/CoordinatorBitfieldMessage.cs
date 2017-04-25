using System;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorBitfieldMessage : SenderMessage
    {
        private readonly byte[] data;

        public CoordinatorBitfieldMessage(Bitfield bitfield)
        {
            data = new byte[(bitfield.Length - 1) / 8 + 1];

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (bitfield[i])
                {
                    data[i / 8] += (byte)(1 << (byte)(7 - i % 8));
                }
            }
        }

        public string Type
        {
            get { return "bitfield"; }
        }

        public NetworkOutgoingMessage Apply(byte id)
        {
            return new Instance(id, data);
        }

        public void Release()
        {
        }

        private class Instance : NetworkOutgoingMessage
        {
            private readonly byte id;
            private readonly byte[] data;

            public Instance(byte id, byte[] data)
            {
                this.id = id;
                this.data = data;
            }

            public int Length
            {
                get { return data.Length + 5; }
            }

            public void ToBytes(DataBlock block)
            {
                block.With((buffer, offset, count) =>
                {
                    buffer[offset + 4] = id;

                    Bytes.Write(data.Length + 1, buffer, offset);
                    Array.Copy(data, 0, buffer, offset + 5, data.Length);
                });
            }

            public void Release()
            {
            }
        }
    }
}