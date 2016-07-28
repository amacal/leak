using Leak.Core.Network;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopCompositeMessage : NetworkOutgoingMessage
    {
        private readonly NetworkOutgoingMessage[] messages;
        private readonly int length;

        public ConnectionLoopCompositeMessage(NetworkOutgoingMessage[] messages)
        {
            this.messages = messages;

            foreach (NetworkOutgoingMessage message in messages)
            {
                length += message.Length;
            }
        }

        public int Length
        {
            get { return length; }
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[length];
            int position = 0;

            foreach (NetworkOutgoingMessage message in messages)
            {
                Array.Copy(message.ToBytes(), 0, data, position, message.Length);
                position += message.Length;
            }

            return data;
        }
    }
}