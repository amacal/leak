using System.Net;

namespace Leak.Tracker.Get.Events
{
    public class TrackerPacketIgnored
    {
        public IPEndPoint Endpoint;
        public int Size;
    }
}