using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Omnibus
{
    public class OmnibusStrategyContext
    {
        /// <summary>
        /// The peer the strategy should find blocks for.
        /// </summary>
        public PeerHash Peer { get; set; }

        /// <summary>
        /// The peer's bitfield the strategy should find blocks for.
        /// </summary>
        public Bitfield Bitfield { get; set; }

        public OmnibusPieceCollection Pieces { get; set; }

        public OmnibusReservationCollection Reservations { get; set; }

        public OmnibusConfiguration Configuration { get; set; }
    }
}