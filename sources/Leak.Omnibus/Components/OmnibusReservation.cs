using System;
using Leak.Common;

namespace Leak.Omnibus.Components
{
    public class OmnibusReservation
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public OmnibusBlock Request { get; set; }
    }
}