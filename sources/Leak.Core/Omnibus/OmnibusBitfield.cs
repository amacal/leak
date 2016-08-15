using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Omnibus
{
    /// <summary>
    /// Manages the global bitfield for a given FileHash. It manages all peers
    /// which announced their bitfield, tracks all requested pieces/blocks and
    /// monitors completed pieces.
    /// </summary>
    public class OmnibusBitfield
    {
        private readonly object synchronized;
        private readonly OmnibusConfiguration configuration;
        private readonly OmnibusBitfieldCollection bitfields;
        private readonly OmnibusPieceCollection pieces;
        private readonly OmnibusReservationCollection reservations;

        public OmnibusBitfield(OmnibusConfiguration configuration)
        {
            this.configuration = configuration;
            this.synchronized = new object();

            this.pieces = new OmnibusPieceCollection(configuration);
            this.bitfields = new OmnibusBitfieldCollection();
            this.reservations = new OmnibusReservationCollection();
        }

        public OmnibusBitfield(OmnibusBitfield bitfield, OmnibusConfiguration configuration)
        {
            this.configuration = configuration;
            this.pieces = new OmnibusPieceCollection(configuration);

            this.synchronized = bitfield.synchronized;
            this.bitfields = bitfield.bitfields;
            this.reservations = bitfield.reservations;
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            lock (synchronized)
            {
                bitfields.Add(peer, bitfield);
            }
        }

        /// <summary>
        /// Reports completeness of the local bitfield.
        /// </summary>
        /// <param name="bitfield">The local bitfield.</param>
        public void Complete(Bitfield bitfield)
        {
            lock (synchronized)
            {
                pieces.Reduce(bitfield.Length);

                for (int i = 0; i < bitfield.Length; i++)
                {
                    if (bitfield[i])
                    {
                        pieces.Complete(i);
                    }
                }
            }
        }

        /// <summary>
        /// Reports completeness of the received block.
        /// </summary>
        /// <param name="block">The received block structure.</param>
        /// <returns>The value indicated whether the block completed also the piece.</returns>
        public bool Complete(OmnibusBlock block)
        {
            lock (synchronized)
            {
                reservations.Complete(block);

                return pieces.Complete(block.Piece, block.Offset / configuration.BlockSize);
            }
        }

        /// <summary>
        /// Invalidates the entire piece.
        /// </summary>
        /// <param name="piece">The piece index to invalidate.</param>
        public void Invalidate(int piece)
        {
            lock (synchronized)
            {
                pieces.Invalidate(piece);
            }
        }

        public bool IsComplete()
        {
            lock (synchronized)
            {
                return pieces.IsComplete();
            }
        }

        public bool IsComplete(int piece)
        {
            lock (synchronized)
            {
                return pieces.IsComplete(piece);
            }
        }

        public bool IsComplete(PeerHash peer)
        {
            lock (synchronized)
            {
                return bitfields.IsComplete(peer);
            }
        }

        public IEnumerable<OmnibusBlock> Next(OmnibusStrategy strategy, PeerHash peer, int count)
        {
            lock (synchronized)
            {
                if (bitfields.Contains(peer))
                {
                    OmnibusStrategyContext context = new OmnibusStrategyContext
                    {
                        Peer = peer,
                        Bitfield = bitfields.ByPeer(peer),
                        Pieces = pieces,
                        Configuration = configuration,
                        Reservations = reservations
                    };

                    return strategy.Next(context, count);
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
            lock (synchronized)
            {
                return reservations.Add(peer, request);
            }
        }
    }
}