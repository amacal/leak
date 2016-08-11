namespace Leak.Core.Congestion
{
    public class PeerCongestionState
    {
        public PeerCongestionState()
        {
            IsChoking = true;
            IsInterested = false;
        }

        public bool IsChoking { get; set; }

        public bool IsInterested { get; set; }
    }
}