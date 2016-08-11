namespace Leak.Core.Congestion
{
    public static class PeerCongestionExtensions
    {
        public static PeerCongestionState GetState(this PeerCongestionEntry entry, PeerCongestionDirection direction)
        {
            return direction == PeerCongestionDirection.Local ? entry.Local : entry.Remote;
        }
    }
}