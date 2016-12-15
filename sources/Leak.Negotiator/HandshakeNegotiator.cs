using Leak.Common;

namespace Leak.Negotiator
{
    public interface HandshakeNegotiator
    {
        void Start(NetworkConnection connection, HandshakeNegotiatorActiveContext context);

        void Handle(NetworkConnection connection, HandshakeNegotiatorPassiveContext context);
    }
}