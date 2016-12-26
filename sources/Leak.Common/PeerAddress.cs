using System.Net;

namespace Leak.Common
{
    public class PeerAddress
    {
        private readonly string host;
        private readonly int port;

        public PeerAddress(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public static PeerAddress Parse(IPEndPoint remote)
        {
            return new PeerAddress(remote.Address.ToString(), remote.Port);
        }

        public static PeerAddress Parse(IPAddress address, int port)
        {
            return new PeerAddress(address.ToString(), port);
        }

        public string Host
        {
            get { return host; }
        }

        public int Port
        {
            get { return port; }
        }

        public override string ToString()
        {
            return $"{host}:{port}";
        }

        public override int GetHashCode()
        {
            return host.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            PeerAddress other = obj as PeerAddress;

            return other != null && other.host == host && other.port == port;
        }
    }
}