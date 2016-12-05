using Leak.Common;

namespace Leak.Core.Cando.PeerExchange
{
    public class PeerExchangeData
    {
        private readonly PeerAddress[] added;

        public PeerExchangeData(PeerAddress[] added)
        {
            this.added = added;
        }

        public PeerAddress[] Added
        {
            get { return added; }
        }
    }
}