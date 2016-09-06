using Leak.Core.Common;

namespace Leak.Core.Omnibus.Events
{
    public class OmnibusReservationEvent
    {
        private readonly PeerHash peer;
        private readonly OmnibusBlock[] blocks;

        public OmnibusReservationEvent(PeerHash peer, OmnibusBlock[] blocks)
        {
            this.peer = peer;
            this.blocks = blocks;
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public int Count
        {
            get { return blocks.Length; }
        }

        public OmnibusBlock[] Blocks
        {
            get { return blocks; }
        }
    }
}