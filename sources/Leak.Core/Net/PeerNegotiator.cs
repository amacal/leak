namespace Leak.Core.Net
{
    public interface PeerNegotiator
    {
        void Active(PeerNegotiatorActiveContext context);

        void Passive(PeerNegotiatorPassiveContext context);
    }
}