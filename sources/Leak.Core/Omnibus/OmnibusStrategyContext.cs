using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Omnibus
{
    public class OmnibusStrategyContext
    {
        public PeerHash Peer { get; set; }

        public Bitfield Bitfield { get; set; }

        public OmnibusPieceCollection Completed { get; set; }

        public OmnibusReservationCollection Reservations { get; set; }

        public OmnibusConfiguration Configuration { get; set; }
    }
}