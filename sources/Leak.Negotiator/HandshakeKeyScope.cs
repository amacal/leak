using System;

namespace Leak.Peer.Negotiator
{
    public class HandshakeKeyScope : IDisposable
    {
        private readonly HandshakeKey key;

        public HandshakeKeyScope(HandshakeKey key)
        {
            this.key = key;
        }

        public void Dispose()
        {
        }
    }
}