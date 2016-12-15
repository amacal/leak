using System;

namespace Leak.Negotiator
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