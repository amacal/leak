using Geon;
using Leak.Core.Common;
using System.Linq;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerGeolocator
    {
        private readonly Geo geo;
        private readonly string[] accept;

        public PeerBouncerGeolocator(Geo geo, string[] accept)
        {
            this.geo = geo;
            this.accept = accept;
        }

        public bool Verify(PeerAddress address)
        {
            string country = geo.Find(address.Host)?.Code;
            bool accepts = country != null && accept.Contains(country);

            return accepts;
        }
    }
}