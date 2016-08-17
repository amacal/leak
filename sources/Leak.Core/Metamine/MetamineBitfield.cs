using Leak.Core.Common;
using System;
using System.Linq;

namespace Leak.Core.Metamine
{
    /// <summary>
    /// Manages the bitfield for a metadata of a given FileHash. It manages
    /// all peers which returned the metadata blocks, tracks all requested
    /// blocks and monitors completeness.
    /// </summary>
    public class MetamineBitfield
    {
        private readonly object synchronized;
        private readonly MetamineConfiguration configuration;
        private readonly MetamineBlockCollection blocks;
        private readonly MetamineReservationCollection reservations;

        public MetamineBitfield(Action<MetamineConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
            });

            synchronized = new object();
            blocks = new MetamineBlockCollection();
            reservations = new MetamineReservationCollection();
        }

        public MetamineBlock[] Next(MetamineStrategy strategy, PeerHash peer)
        {
            lock (synchronized)
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
        }

        public PeerHash Reserve(PeerHash peer, MetamineBlock block)
        {
            lock (synchronized)
            {
                return reservations.Add(peer, block);
            }
        }

        public void Complete(MetamineBlock block)
        {
            lock (synchronized)
            {
                blocks.Complete(block);
                reservations.Complete(block);
            }
        }
    }
}