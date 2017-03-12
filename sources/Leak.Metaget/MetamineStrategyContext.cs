using Leak.Common;

namespace Leak.Meta.Get
{
    public class MetamineStrategyContext
    {
        /// <summary>
        /// The peer the strategy should find blocks for.
        /// </summary>
        public PeerHash Peer { get; set; }

        public MetamineBlockCollection Blocks { get; set; }

        public MetamineReservationCollection Reservations { get; set; }

        public MetamineConfiguration Configuration { get; set; }
    }
}