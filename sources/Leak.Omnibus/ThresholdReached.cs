using Leak.Common;

namespace Leak.Data.Map
{
    public class ThresholdReached
    {
        public FileHash Hash;
        public PeerHash Peer;

        public int Threshold;
        public int Value;
    }
}