using Leak.Core.Common;
using System;

namespace Leak.Core.Omnibus
{
    public class OmnibusReservation
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public OmnibusBlock Request { get; set; }
    }
}