namespace Leak.Core.Extensions.PeerExchange
{
    public class PeerExchangeMessage
    {
        private readonly PeerExchangePeer[] added;

        public PeerExchangeMessage(PeerExchangePeer[] added)
        {
            this.added = added;
        }

        public PeerExchangePeer[] Added
        {
            get { return added; }
        }
    }
}