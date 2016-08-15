using Leak.Core.Common;
using System;

namespace Leak.Core.Metamine
{
    public class MetamineReservation
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public MetamineBlock Request { get; set; }
    }
}