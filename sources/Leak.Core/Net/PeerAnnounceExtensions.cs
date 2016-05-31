using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Leak.Core.Net
{
    public static class PeerAnnounceExtensions
    {
        public static void SetAddress(this PeerAnnounceConfiguration configuration, IPAddress ip)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                configuration.Address = ip.GetAddressBytes();
            }
        }

        public static void SetAddress(this PeerAnnounceConfiguration configuration, string hostOrAddress)
        {
            configuration.Address = hostOrAddress.Split('.').Select(Byte.Parse).ToArray();
        }
    }
}