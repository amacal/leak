namespace Leak.Core.Extensions.PeerExchange
{
    public class PeerExchangePeer
    {
        private readonly string host;
        private readonly int port;

        public PeerExchangePeer(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public string Host
        {
            get { return host; }
        }

        public int Port
        {
            get { return port; }
        }
    }
}