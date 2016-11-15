using Leak.Core.Common;
using Leak.Core.Network;
using System;

namespace Leak.Core.Glue
{
    public class GlueEntry
    {
        public long Identifier;

        public PeerHash Peer;

        public PeerAddress Remote;

        public NetworkDirection Direction;

        public Bitfield Bitfield;

        public GlueState State;

        public DateTime Timestamp;

        public GlueMore More;
    }
}