using System.Linq;
using Geon;
using Geon.Formats;
using Geon.Readers;
using Geon.Sources;
using Leak.Client.Swarm;
using Leak.Common;

namespace Leak
{
    public class GeonFilter : SwarmFilter
    {
        private readonly string[] acceptable;
        private readonly Geo geo;

        public GeonFilter(string[] acceptable)
        {
            this.acceptable = acceptable;

            geo = GeoFactory.Open(with =>
            {
                with.Source(new MaxMindSource());
                with.Format(new ZipFormat());
                with.Reader(new CsvReader());
            });
        }

        public bool Accept(PeerAddress address)
        {
            return acceptable.Contains(geo.Find(address.Host)?.Code);
        }
    }
}