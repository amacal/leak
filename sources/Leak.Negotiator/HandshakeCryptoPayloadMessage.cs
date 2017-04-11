using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeCryptoPayloadMessage : NetworkOutgoingMessage
    {
        private readonly int method;

        public HandshakeCryptoPayloadMessage(int method)
        {
            this.method = method;
        }

        public int Length
        {
            get { return 14; }
        }

        public void ToBytes(DataBlock block)
        {
            byte[] data = Bytes.Concatenate(HandshakeCryptoPayload.GetVerification(), Bytes.ToInt32(method), Bytes.Parse("0000"));

            block.With((buffer, offset, count) =>
            {
                Array.Copy(data, 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}