using Leak.Common;
using Leak.Communicator;
using Leak.Extensions;
using Leak.Loop;
using System;

namespace Leak.Glue
{
    public class GlueEntry
    {
        public long Identifier;

        public PeerHash Peer;

        public PeerAddress Remote;

        public NetworkDirection Direction;

        public bool Extensions;

        public Bitfield Bitfield;

        public PeerState State;

        public DateTime Timestamp;

        public MoreContainer More;

        public ConnectionLoop Loopy;

        public CommunicatorService Commy;
    }
}