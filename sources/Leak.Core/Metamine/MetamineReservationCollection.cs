﻿using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Metamine
{
    public class MetamineReservationCollection
    {
        private readonly Dictionary<MetamineBlock, MetamineReservation> byBlock;
        private readonly Dictionary<PeerHash, HashSet<MetamineReservation>> byPeer;

        public MetamineReservationCollection()
        {
            byBlock = new Dictionary<MetamineBlock, MetamineReservation>();
            byPeer = new Dictionary<PeerHash, HashSet<MetamineReservation>>();
        }

        public bool Contains(MetamineBlock request, DateTime now)
        {
            MetamineReservation book;

            if (byBlock.TryGetValue(request, out book) == false)
                return false;

            return book.Expires > now;
        }

        public bool Contains(MetamineBlock request, PeerHash peer)
        {
            MetamineReservation book;

            if (byBlock.TryGetValue(request, out book) == false)
                return false;

            return book.Peer.Equals(peer);
        }

        public PeerHash Add(PeerHash peer, MetamineBlock request)
        {
            PeerHash previous = null;
            MetamineReservation book;

            if (byBlock.TryGetValue(request, out book))
            {
                previous = book.Peer;
            }

            byBlock[request] = new MetamineReservation
            {
                Peer = peer,
                Expires = DateTime.Now.AddSeconds(30),
                Request = request
            };

            if (byPeer.ContainsKey(peer) == false)
            {
                byPeer.Add(peer, new HashSet<MetamineReservation>());
            }

            byPeer[peer].Add(byBlock[request]);
            return previous;
        }

        public void Complete(MetamineBlock request)
        {
            MetamineReservation block;
            byBlock.TryGetValue(request, out block);

            if (block != null)
            {
                byPeer[block.Peer].Remove(block);
                byBlock.Remove(request);
            }
        }
    }
}