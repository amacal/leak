using System;
using System.Linq;
using Leak.Common;
using Leak.Tasks;

namespace Leak.Meta.Get
{
    /// <summary>
    /// Manages the bitfield for a metadata of a given FileHash. It manages
    /// all peers which returned the metadata blocks, tracks all requested
    /// blocks and monitors completeness.
    /// </summary>
    public class MetamineBitfield
    {
        private readonly MetamineConfiguration configuration;
        private readonly MetamineBlockCollection blocks;
        private readonly MetamineReservationCollection reservations;

        public MetamineBitfield(Action<MetamineConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
            });

            blocks = new MetamineBlockCollection();
            reservations = new MetamineReservationCollection();
        }

        public MetamineBlock[] Next(MetamineStrategy strategy, PeerHash peer)
        {
            MetamineStrategyContext context = new MetamineStrategyContext
            {
                Peer = peer,
                Blocks = blocks,
                Configuration = configuration,
                Reservations = reservations
            };

            return strategy.Next(context).ToArray();
        }

        public PeerHash Reserve(PeerHash peer, MetamineBlock block)
        {
            return reservations.Add(peer, block);
        }

        public void Complete(MetamineBlock block)
        {
            blocks.Complete(block);
            reservations.Complete(block);
        }
    }
}