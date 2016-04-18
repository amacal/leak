namespace Leak.Core.IO
{
    public class MetainfoFileTracker
    {
        private readonly string uri;

        public MetainfoFileTracker(string uri)
        {
            this.uri = uri;
        }

        public string Uri
        {
            get { return uri; }
        }

        public MetainfoFileTrackerProtocol Protocol
        {
            get
            {
                if (uri.StartsWith("udp://"))
                    return MetainfoFileTrackerProtocol.Udp;

                if (uri.StartsWith("http://"))
                    return MetainfoFileTrackerProtocol.Http;

                if (uri.StartsWith("https://"))
                    return MetainfoFileTrackerProtocol.Https;

                return MetainfoFileTrackerProtocol.Unknown;
            }
        }
    }
}