using System;
using Leak.Common;

namespace Leak.Data.Map.Components
{
    public class OmnibusReservation
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public BlockIndex Request { get; set; }
    }
}