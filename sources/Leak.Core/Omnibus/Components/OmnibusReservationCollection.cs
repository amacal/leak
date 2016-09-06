using System;
using System.Collections.Generic;
using Leak.Core.Common;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusReservationCollection
    {
        private readonly Dictionary<OmnibusBlock, OmnibusReservation> byBlock;
        private readonly Dictionary<PeerHash, HashSet<OmnibusReservation>> byPeer;

        public OmnibusReservationCollection()
        {
            byBlock = new Dictionary<OmnibusBlock, OmnibusReservation>();
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

        public PeerHash Add(PeerHash peer, OmnibusBlock request)
        {
            PeerHash previous = null;
            OmnibusReservation book;

            if (byBlock.TryGetValue(request, out book))
            {
                previous = book.Peer;
            }

            byBlock[request] = new OmnibusReservation
            {
                Peer = peer,
                Expires = DateTime.Now.AddSeconds(30),
                Request = request
            };

            if (byPeer.ContainsKey(peer) == false)
            {
                byPeer.Add(peer, new HashSet<OmnibusReservation>());
            }

            byPeer[peer].Add(byBlock[request]);
            return previous;
        }

        public void Complete(OmnibusBlock request)
        {
            OmnibusReservation block;
            byBlock.TryGetValue(request, out block);

            if (block != null)
            {
                byPeer[block.Peer].Remove(block);
                byBlock.Remove(request);
            }
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