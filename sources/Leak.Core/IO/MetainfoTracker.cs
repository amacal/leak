using System;

namespace Leak.Core.IO
{
    public class MetainfoTracker
    {
        private readonly Uri uri;

        public MetainfoTracker(Uri uri)
        {
            this.uri = uri;
        }

        public Uri Uri
        {
            get { return uri; }
        }

        public MetainfoTrackerProtocol Protocol
        {
            get
            {
                if (uri.Scheme.Equals("udp", StringComparison.OrdinalIgnoreCase))
                    return MetainfoTrackerProtocol.Udp;

                if (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
                    return MetainfoTrackerProtocol.Http;

                return MetainfoTrackerProtocol.Unknown;
            }
        }
    }
}