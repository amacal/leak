using Leak.Networking;

namespace Leak.Negotiator
{
    public class HandshakeNegotiatorFactory
    {
        private readonly NetworkPool pool;

        public HandshakeNegotiatorFactory(NetworkPool pool)
        {
            this.pool = pool;
        }

        public HandshakeNegotiator Create(HandshakeNegotiatorHooks hooks)
        {
            return new HandshakeNegotiatorInstance(pool, hooks);
        }
    }
}