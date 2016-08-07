using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    public class OmnibusReservationCollections
    {
        private readonly Dictionary<OmnibusBlock, OmnibusReservation> blocks;
        private readonly Dictionary<PeerHash, HashSet<OmnibusReservation>> byPeer;

        public OmnibusReservationCollections()
        {
            this.blocks = new Dictionary<OmnibusBlock, OmnibusReservation>();
            this.byPeer = new Dictionary<PeerHash, HashSet<OmnibusReservation>>();
        }

        public bool Contains(OmnibusBlock request, DateTime now)
        {
            OmnibusReservation book;

            if (blocks.TryGetValue(request, out book) == false)
                return false;

            return book.Expires > now;
        }

        public bool Contains(OmnibusBlock request, PeerHash peer)
        {
            OmnibusReservation book;

            if (blocks.TryGetValue(request, out book) == false)
                return false;

            return book.Peer.Equals(peer);
        }

        public PeerHash Add(PeerHash peer, OmnibusBlock request)
        {
            PeerHash previous = null;
            OmnibusReservation book;

            if (blocks.TryGetValue(request, out book) == true)
            {
                previous = book.Peer;
            }

            blocks[request] = new OmnibusReservation
            {
                Peer = peer,
                Expires = DateTime.Now.AddSeconds(30),
                Request = request
            };

            if (byPeer.ContainsKey(peer) == false)
            {
                byPeer.Add(peer, new HashSet<OmnibusReservation>());
            }

            byPeer[peer].Add(blocks[request]);
            return previous;
        }

        public void Complete(OmnibusBlock request)
        {
            OmnibusReservation block;
            blocks.TryGetValue(request, out block);

            if (block != null)
            {
                byPeer[block.Peer].Remove(block);
                blocks.Remove(request);
            }
        }

        public int Count(PeerHash peer)
        {
            int count = 0;
            HashSet<OmnibusReservation> books;
            byPeer.TryGetValue(peer, out books);

            if (books == null)
                return 0;

            foreach (OmnibusReservation book in books)
            {
                count++;
            }

            return count;
        }
    }
}