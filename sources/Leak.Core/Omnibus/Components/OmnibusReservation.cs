using System;
using Leak.Core.Common;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusReservation
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public OmnibusBlock Request { get; set; }
    }
}