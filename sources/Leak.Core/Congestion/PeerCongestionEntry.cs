namespace Leak.Core.Congestion
{
    public class PeerCongestionEntry
    {
        public PeerCongestionEntry()
        {
            Local = new PeerCongestionState();
            Remote = new PeerCongestionState();
        }

        public PeerCongestionState Local { get; set; }

        public PeerCongestionState Remote { get; set; }
    }
}