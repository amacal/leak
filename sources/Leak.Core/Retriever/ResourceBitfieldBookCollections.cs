using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceBitfieldBookCollections
    {
        private readonly Dictionary<ResourceBlock, ResourceBitfieldBook> blocks;
        private readonly Dictionary<PeerHash, HashSet<ResourceBitfieldBook>> byPeer;

        public ResourceBitfieldBookCollections()
        {
            this.blocks = new Dictionary<ResourceBlock, ResourceBitfieldBook>();
            this.byPeer = new Dictionary<PeerHash, HashSet<ResourceBitfieldBook>>();
        }

        public bool Contains(ResourceBlock request, DateTime now)
        {
            ResourceBitfieldBook book;

            if (blocks.TryGetValue(request, out book) == false)
                return false;

            return book.Expires > now;
        }

        public bool Contains(ResourceBlock request, PeerHash peer)
        {
            ResourceBitfieldBook book;

            if (blocks.TryGetValue(request, out book) == false)
                return false;

            return book.Peer.Equals(peer);
        }

        public PeerHash Add(PeerHash peer, ResourceBlock request)
        {
            PeerHash previous = null;
            ResourceBitfieldBook book;

            if (blocks.TryGetValue(request, out book) == true)
            {
                previous = book.Peer;
            }

            blocks[request] = new ResourceBitfieldBook
            {
                Peer = peer,
                Expires = DateTime.Now.AddSeconds(30),
                Request = request
            };

            if (byPeer.ContainsKey(peer) == false)
            {
                byPeer.Add(peer, new HashSet<ResourceBitfieldBook>());
            }

            byPeer[peer].Add(blocks[request]);
            return previous;
        }

        public void Complete(ResourceBlock request)
        {
            ResourceBitfieldBook block;
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
            HashSet<ResourceBitfieldBook> books;
            byPeer.TryGetValue(peer, out books);

            if (books == null)
                return 0;

            foreach (ResourceBitfieldBook book in books)
            {
                count++;
            }

            return count;
        }
    }
}