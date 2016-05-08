namespace Leak.Core.Net
{
    public abstract class PeerNegotiator
    {
        public abstract void Active(PeerNegotiatorAware channel);

        public abstract void Passive(PeerNegotiatorAware channel);
    }
}