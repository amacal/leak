using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public class PeerAnnounceConfigurator
    {
        private byte[] address;
        private int? port;

        public void Address(IPAddress ip)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                address = ip.GetAddressBytes();
            }
        }

        public void SetAddress(string hostOrAddress)
        {
            address = hostOrAddress.Split('.').Select(Byte.Parse).ToArray();
            return;

            foreach (IPAddress ip in Dns.GetHostEntry(hostOrAddress).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = ip.GetAddressBytes();
                    break;
                }
            }
        }

        public void SetPort(int value)
        {
            port = value;
        }

        public void Apply(PeerAnnounceConfiguration configuration)
        {
            if (address != null)
                configuration.Address = address;

            if (port != null)
                configuration.Port = port.Value;
        }
    }
}