using System.Collections.Generic;
using Leak.Common;

namespace Leak.Data.Map.Events
{
    public class OmnibusReservationEvent
    {
        private readonly PeerHash peer;
        private readonly IReadOnlyCollection<BlockIndex> blocks;

        public OmnibusReservationEvent(PeerHash peer, IReadOnlyCollection<BlockIndex> blocks)
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
            get { return blocks.Count; }
        }

        public IEnumerable<BlockIndex> Blocks
        {
            get { return blocks; }
        }
    }
}