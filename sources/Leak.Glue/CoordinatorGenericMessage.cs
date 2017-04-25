using System;
using Leak.Networking.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorGenericMessage : SenderMessage
    {
        private readonly string type;

        public CoordinatorGenericMessage(string type)
        {
            this.type = type;
        }

        public string Type
        {
            get { return type; }
        }

        public NetworkOutgoingMessage Apply(byte id)
        {
            return new Instance(id);
        }

        public void Release()
        {
        }

        private class Instance : NetworkOutgoingMessage
        {
            private readonly byte id;

            public Instance(byte id)
            {
                this.id = id;
            }

            public int Length
            {
                get { return 5; }
            }

            public void ToBytes(DataBlock block)
            {
                block.With((buffer, offset, count) =>
                {
                    Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x01, id }, 0, buffer, offset, Length);
                });
            }

            public void Release()
            {
            }
        }
    }
}