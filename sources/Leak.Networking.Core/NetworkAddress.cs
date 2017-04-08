using System.Net;

namespace Leak.Networking.Core
{
    public class NetworkAddress
    {
        private readonly string host;
        private readonly int port;

        public NetworkAddress(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public static NetworkAddress Parse(IPEndPoint remote)
        {
            return new NetworkAddress(remote.Address.ToString(), remote.Port);
        }

        public static NetworkAddress Parse(IPAddress address, int port)
        {
            return new NetworkAddress(address.ToString(), port);
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
            NetworkAddress other = obj as NetworkAddress;

            return other != null && other.host == host && other.port == port;
        }
    }
}