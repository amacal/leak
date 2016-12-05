using System.Collections.Generic;
using Leak.Common;

namespace Leak.Core.Omnibus.Events
{
    public class OmnibusReservationEvent
    {
        private readonly PeerHash peer;
        private readonly IReadOnlyCollection<OmnibusBlock> blocks;

        public OmnibusReservationEvent(PeerHash peer, IReadOnlyCollection<OmnibusBlock> blocks)
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

        public IEnumerable<OmnibusBlock> Blocks
        {
            get { return blocks; }
        }
    }
}