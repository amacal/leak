using Leak.Core.Common;
using Leak.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Omnibus
{
    /// <summary>
    /// Manages the global bitfield for a given FileHash. It manages all peers
    /// which announced their bitfield, tracks all requested pieces/blocks and
    /// monitors completed pieces.
    /// </summary>
    public class OmnibusService
    {
        private readonly OmnibusContext context;

        public OmnibusService(Action<OmnibusConfiguration> configurer)
        {
            context = new OmnibusContext(configurer);
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            lock (context.Synchronized)
            {
                context.Bitfields.Add(peer, bitfield);
            }
        }

        /// <summary>
        /// Reports completeness of the received block.
        /// </summary>
        /// <param name="block">The received block structure.</param>
        /// <returns>The value indicated whether the block completed also the piece.</returns>
        public bool Complete(OmnibusBlock block)
        {
            lock (context.Synchronized)
            {
                context.Reservations.Complete(block);

                return context.Pieces.Complete(block.Piece, block.Offset / context.Metainfo.Properties.BlockSize);
            }
        }

        /// <summary>
        /// Invalidates the entire piece.
        /// </summary>
        /// <param name="piece">The piece index to invalidate.</param>
        public void Invalidate(int piece)
        {
            lock (context.Synchronized)
            {
                context.Pieces.Invalidate(piece);
            }
        }

        public bool IsComplete()
        {
            lock (context.Synchronized)
            {
                return context.Pieces.IsComplete();
            }
        }

        public bool IsComplete(int piece)
        {
            lock (context.Synchronized)
            {
                return context.Pieces.IsComplete(piece);
            }
        }

        public bool IsComplete(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Bitfields.IsComplete(peer);
            }
        }

        public IEnumerable<OmnibusBlock> Next(OmnibusStrategy strategy, PeerHash peer, int count)
        {
            lock (context.Synchronized)
            {
                if (context.Bitfields.Contains(peer))
                {
                    OmnibusStrategyContext inner = new OmnibusStrategyContext
                    {
                        Peer = peer,
                        Pieces = context.Pieces,
                        Metainfo = context.Metainfo,
                        Reservations = context.Reservations,
                        Bitfield = context.Bitfields.ByPeer(peer)
                    };

                    return strategy.Next(inner, count);
                }
            }

            return Enumerable.Empty<OmnibusBlock>();
        }

        /// <summary>
        /// Reserves the block to be downloaded by the given peer.
        /// </summary>
        /// <param name="peer">The peer which reserves the block.</param>
        /// <param name="request">The block structure describing the reservation.</param>
        /// <returns>The optional peer which had assigned reservation.</returns>
        public PeerHash Reserve(PeerHash peer, OmnibusBlock request)
        {
            lock (context.Synchronized)
            {
                return context.Reservations.Add(peer, request);
            }
        }
    }
}