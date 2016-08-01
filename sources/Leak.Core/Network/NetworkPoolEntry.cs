namespace Leak.Core.Network
{
    public class NetworkPoolEntry
    {
        public NetworkPoolConnection Connection { get; set; }

        public bool IsAvailable { get; set; }
    }
}