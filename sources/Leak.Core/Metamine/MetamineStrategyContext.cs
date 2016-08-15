using Leak.Core.Common;

namespace Leak.Core.Metamine
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