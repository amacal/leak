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
        private readonly OmnibusConfiguration configuration;
        private readonly OmnibusBitfieldCollection bitfields;
        private readonly OmnibusPieceCollection completed;
        private readonly OmnibusReservationCollection reservations;

        public OmnibusBitfield(OmnibusConfiguration configuration)
        {
            this.configuration = configuration;
            this.bitfields = new OmnibusBitfieldCollection();
            this.completed = new OmnibusPieceCollection(configuration);
            this.reservations = new OmnibusReservationCollection();
        }

        public OmnibusBitfield(OmnibusBitfield bitfield, OmnibusConfiguration configuration)
        {
            this.configuration = configuration;
            this.completed = new OmnibusPieceCollection(configuration);

            this.bitfields = bitfield.bitfields;
            this.reservations = bitfield.reservations;
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            bitfields.Add(peer, bitfield);
        }

        public void Complete(Bitfield bitfield)
        {
            completed.Reduce(bitfield.Length);

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (bitfield[i])
                {
                    completed.Complete(i);
                }
            }
        }

        public bool Complete(OmnibusBlock block)
        {
            reservations.Complete(block);

            return completed.Complete(block.Piece, block.Offset / configuration.BlockSize);
        }

        public void Invalidate(int piece)
        {
            completed.Invalidate(piece);
        }

        public bool IsComplete()
        {
            return completed.IsComplete();
        }

        public bool IsComplete(int piece)
        {
            return completed.IsComplete(piece);
        }

        public bool IsComplete(PeerHash peer)
        {
            return bitfields.IsComplete(peer);
        }

        public IEnumerable<OmnibusBlock> Next(OmnibusStrategy strategy, PeerHash peer, int count)
        {
            if (bitfields.Contains(peer))
            {
                OmnibusStrategyContext context = new OmnibusStrategyContext
                {
                    Peer = peer,
                    Bitfield = bitfields.ByPeer(peer),
                    Completed = completed,
                    Configuration = configuration,
                    Reservations = reservations
                };

                return strategy.Next(context, count);
            }

            return Enumerable.Empty<OmnibusBlock>();
        }

        public PeerHash Reserve(PeerHash peer, OmnibusBlock request)
        {
            return reservations.Add(peer, request);
        }
    }
}