using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceMetadataBookCollection
    {
        private readonly Dictionary<ResourceMetadataBlock, ResourceMetadataBook> blocks;
        private readonly Dictionary<PeerHash, HashSet<ResourceMetadataBook>> byPeer;

        public ResourceMetadataBookCollection()
        {
            this.blocks = new Dictionary<ResourceMetadataBlock, ResourceMetadataBook>();
            this.byPeer = new Dictionary<PeerHash, HashSet<ResourceMetadataBook>>();
        }

        private int? total;
        private bool complete;

        public void Complete(int size)
        {
            total = size;
            complete = true;
        }

        public void Complete(ResourceMetadataBlock request, int size)
        {
            ResourceMetadataBook book;
            blocks.TryGetValue(request, out book);

            if (book != null)
            {
                total = size;
                byPeer[book.Peer].Remove(book);
            }
        }

        public bool IsComplete()
        {
            return complete == true;
        }

        public bool Contains(ResourceMetadataBlock request)
        {
            return blocks.ContainsKey(request) &&
                   blocks[request].Expires > DateTime.Now;
        }

        public void Add(PeerHash peer, ResourceMetadataBlock request)
        {
            blocks[request] = new ResourceMetadataBook
            {
                Peer = peer,
                Expires = DateTime.Now.AddMinutes(2),
                Request = request
            };

            if (byPeer.ContainsKey(peer) == false)
            {
                byPeer.Add(peer, new HashSet<ResourceMetadataBook>());
            }

            byPeer[peer].Add(blocks[request]);
        }

        public ResourceMetadataBlock[] Next(PeerHash peer, int maximum)
        {
            ResourceMetadataBlock block;
            List<ResourceMetadataBlock> result = new List<ResourceMetadataBlock>();

            if (total == null)
            {
                block = new ResourceMetadataBlock(0);

                if (Contains(block) == false)
                {
                    result.Add(block);
                }
            }
            else
            {
                int position = 0;

                while (position < total)
                {
                    block = new ResourceMetadataBlock(position / 16384);

                    if (Contains(block) == false && result.Count < maximum)
                    {
                        result.Add(block);
                    }

                    position += 16384;
                }
            }

            return result.ToArray();
        }
    }
}