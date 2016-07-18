namespace Leak.Core.Tracker
{
    public class TrackerClientFactory
    {
        public static TrackerClient Create(string uri)
        {
            if (uri.StartsWith("http://"))
                return new TrackerClientToHttp(uri);

            return null;
        }
    }
}