using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceBitfieldBookCollections
    {
        private readonly Dictionary<ResourceBlock, ResourceBitfieldBook> items;
        private readonly Dictionary<PeerHash, HashSet<ResourceBitfieldBook>> byPeer;

        public ResourceBitfieldBookCollections()
        {
            this.items = new Dictionary<ResourceBlock, ResourceBitfieldBook>();
            this.byPeer = new Dictionary<PeerHash, HashSet<ResourceBitfieldBook>>();
        }

        public bool Contains(ResourceBlock request)
        {
            return items.ContainsKey(request) &&
                   items[request].Expires > DateTime.Now;
        }

        public void Add(PeerHash peer, ResourceBlock request)
        {
            items[request] = new ResourceBitfieldBook
            {
                Peer = peer,
                Expires = DateTime.Now.AddMinutes(2),
                Request = request
            };

            if (byPeer.ContainsKey(peer) == false)
            {
                byPeer.Add(peer, new HashSet<ResourceBitfieldBook>());
            }

            byPeer[peer].Add(items[request]);
        }

        public void Complete(ResourceBlock request)
        {
            ResourceBitfieldBook block;
            items.TryGetValue(request, out block);

            if (block != null)
            {
                byPeer[block.Peer].Remove(block);
                items.Remove(request);
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
                if (book.Expires > DateTime.Now)
                {
                    count++;
                }
            }

            return count;
        }
    }
}