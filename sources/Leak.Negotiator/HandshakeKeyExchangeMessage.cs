using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeKeyExchangeMessage : NetworkOutgoingMessage
    {
        private readonly HandshakeCredentials credentials;

        public HandshakeKeyExchangeMessage(HandshakeCredentials credentials)
        {
            this.credentials = credentials;
        }

        public int Length
        {
            get { return credentials.PublicKey.Length + credentials.Padding.Length; }
        }

        public void ToBytes(DataBlock block)
        {
            byte[] data = Bytes.Concatenate(credentials.PublicKey, credentials.Padding);

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