using System;
using Leak.Common;

namespace Leak.Metaget
{
    public class MetamineReservation
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public MetamineBlock Request { get; set; }
    }
}