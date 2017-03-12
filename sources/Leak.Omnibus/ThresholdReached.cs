using Leak.Common;

namespace Leak.Datamap
{
    public class ThresholdReached
    {
        public FileHash Hash;
        public PeerHash Peer;

        public int Threshold;
        public int Value;
    }
}