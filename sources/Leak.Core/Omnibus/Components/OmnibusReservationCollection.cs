using Leak.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusReservationCollection
    {
        private readonly TimeSpan lease;
        private readonly OmnibusReservationComparer comparer;

        private readonly Dictionary<OmnibusBlock, OmnibusReservation> byBlock;
        private readonly Dictionary<PeerHash, HashSet<OmnibusReservation>> byPeer;

        public OmnibusReservationCollection(TimeSpan lease)
        {
            this.lease = lease;

            comparer = new OmnibusReservationComparer();
            byBlock = new Dictionary<OmnibusBlock, OmnibusReservation>(comparer);
            byPeer = new Dictionary<PeerHash, HashSet<OmnibusReservation>>();
        }

        public bool Contains(OmnibusBlock request, DateTime now)
        {
            OmnibusReservation book;

            if (byBlock.TryGetValue(request, out book) == false)
                return false;

            return book.Expires > now;
        }

        public bool Contains(OmnibusBlock request, PeerHash peer)
        {
            OmnibusReservation book;

            if (byBlock.TryGetValue(request, out book) == false)
                return false;

            return book.Peer.Equals(peer);
        }

        public PeerHash Add(PeerHash peer, OmnibusBlock request, DateTime now)
        {
            PeerHash previous = null;
            OmnibusReservation book;
            HashSet<OmnibusReservation> books;

            if (byBlock.TryGetValue(request, out book))
            {
                previous = book.Peer;
            }

            book = new OmnibusReservation
            {
                Peer = peer,
                Expires = now + lease,
                Request = request
            };

            if (byPeer.TryGetValue(peer, out books) == false)
            {
                books = new HashSet<OmnibusReservation>();
                byPeer.Add(peer, books);
            }

            byBlock[request] = book;
            books.Add(book);

            return previous;
        }

        public int Complete(OmnibusBlock request, out PeerHash peer)
        {
            OmnibusReservation block;
            byBlock.TryGetValue(request, out block);

            if (block != null)
            {
                HashSet<OmnibusReservation> blocks;
                byPeer.TryGetValue(block.Peer, out blocks);

                blocks.Remove(block);
                byBlock.Remove(request);

                peer = block.Peer;
                return blocks.Count;
            }

            peer = null;
            return 0;
        }

        public int Count(PeerHash peer)
        {
            HashSet<OmnibusReservation> books;
            byPeer.TryGetValue(peer, out books);

            if (books == null)
                return 0;

            return books.Count;
        }
    }
}