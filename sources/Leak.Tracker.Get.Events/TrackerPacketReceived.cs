using System.Net;

namespace Leak.Tracker.Get.Events
{
    public class TrackerPacketReceived
    {
        public IPEndPoint Endpoint;
        public int Size;
    }
}