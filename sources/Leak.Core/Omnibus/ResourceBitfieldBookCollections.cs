using System;
using System.Collections.Generic;
using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldBookCollections
    {
        private readonly Dictionary<ResourceBlock, OmnibusBitfieldBook> blocks;
        private readonly Dictionary<PeerHash, HashSet<OmnibusBitfieldBook>> byPeer;

        public OmnibusBitfieldBookCollections()
        {
            this.blocks = new Dictionary<ResourceBlock, OmnibusBitfieldBook>();
            this.byPeer = new Dictionary<PeerHash, HashSet<OmnibusBitfieldBook>>();
        }

        public bool Contains(ResourceBlock request, DateTime now)
        {
            OmnibusBitfieldBook book;

            if (blocks.TryGetValue(request, out book) == false)
                return false;

            return book.Expires > now;
        }

        public bool Contains(ResourceBlock request, PeerHash peer)
        {
            OmnibusBitfieldBook book;

            if (blocks.TryGetValue(request, out book) == false)
                return false;

            return book.Peer.Equals(peer);
        }

        public PeerHash Add(PeerHash peer, ResourceBlock request)
        {
            PeerHash previous = null;
            OmnibusBitfieldBook book;

            if (blocks.TryGetValue(request, out book) == true)
            {
                previous = book.Peer;
            }

            blocks[request] = new OmnibusBitfieldBook
            {
                Peer = peer,
                Expires = DateTime.Now.AddSeconds(30),
                Request = request
            };

            if (byPeer.ContainsKey(peer) == false)
            {
                byPeer.Add(peer, new HashSet<OmnibusBitfieldBook>());
            }

            byPeer[peer].Add(blocks[request]);
            return previous;
        }

        public void Complete(ResourceBlock request)
        {
            OmnibusBitfieldBook block;
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
            HashSet<OmnibusBitfieldBook> books;
            byPeer.TryGetValue(peer, out books);

            if (books == null)
                return 0;

            foreach (OmnibusBitfieldBook book in books)
            {
                count++;
            }

            return count;
        }
    }
}