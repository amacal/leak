using Leak.Common;
using Leak.Networking;

namespace Leak.Negotiator
{
    public class HandshakeNegotiatorInstance : HandshakeNegotiator
    {
        private readonly NetworkPool pool;
        private readonly HandshakeNegotiatorHooks hooks;

        public HandshakeNegotiatorInstance(NetworkPool pool, HandshakeNegotiatorHooks hooks)
        {
            this.pool = pool;
            this.hooks = hooks;
        }

        public void Start(NetworkConnection connection, HandshakeNegotiatorActiveContext context)
        {
            new HandshakeNegotiatorActive(pool, connection, context, hooks).Execute();
        }

        public void Handle(NetworkConnection connection, HandshakeNegotiatorPassiveContext context)
        {
            new HandshakeNegotiatorPassive(pool, connection, context, hooks).Execute();
        }
    }
}