namespace Leak.Core.IO
{
    public class MetainfoTracker
    {
        private readonly string uri;

        public MetainfoTracker(string uri)
        {
            this.uri = uri;
        }

        public string Uri
        {
            get { return uri; }
        }

        public MetainfoTrackerProtocol Protocol
        {
            get
            {
                if (uri.StartsWith("udp://"))
                    return MetainfoTrackerProtocol.Udp;

                if (uri.StartsWith("http://"))
                    return MetainfoTrackerProtocol.Http;

                if (uri.StartsWith("https://"))
                    return MetainfoTrackerProtocol.Https;

                return MetainfoTrackerProtocol.Unknown;
            }
        }
    }
}