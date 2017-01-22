using Leak.Common;
using System;

namespace Leak.Negotiator.Tests
{
    public class NegotiatorFixturePair : IDisposable
    {
        private readonly NetworkConnection local;
        private readonly NetworkConnection remote;

        public NegotiatorFixturePair(NetworkConnection local, NetworkConnection remote)
        {
            this.local = local;
            this.remote = remote;
        }

        public NetworkConnection Local
        {
            get { return local; }
        }

        public NetworkConnection Remote
        {
            get { return remote; }
        }

        public void Dispose()
        {
            local?.Terminate();
            remote?.Terminate();
        }
    }
}