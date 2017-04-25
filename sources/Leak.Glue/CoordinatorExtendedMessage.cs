using System;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorExtendedMessage : SenderMessage
    {
        private readonly Extended extended;

        public CoordinatorExtendedMessage(Extended extended)
        {
            this.extended = extended;
        }

        public string Type
        {
            get { return "extended"; }
        }

        public NetworkOutgoingMessage Apply(byte id)
        {
            return new Instance(id, extended);
        }

        public void Release()
        {
        }

        private class Instance : NetworkOutgoingMessage
        {
            private readonly byte id;
            private readonly Extended extended;

            public Instance(byte id, Extended extended)
            {
                this.id = id;
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
                    buffer[offset + 4] = id;
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
}