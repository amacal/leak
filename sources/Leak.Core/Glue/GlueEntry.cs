using Leak.Common;
using Leak.Core.Communicator;
using Leak.Core.Loop;
using System;

namespace Leak.Core.Glue
{
    public class GlueEntry
    {
        public long Identifier;

        public PeerHash Peer;

        public PeerAddress Remote;

        public NetworkDirection Direction;

        public bool Extensions;

        public Bitfield Bitfield;

        public GlueState State;

        public DateTime Timestamp;

        public GlueMore More;

        public ConnectionLoop Loopy;

        public CommunicatorService Commy;
    }
}