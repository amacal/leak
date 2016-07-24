using Leak.Core.Common;
using Leak.Core.Messages;
using System;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceBitfield
    {
        private readonly ResourceStorageConfiguration configuration;
        private readonly ResourceBitfieldMap completed;
        private readonly Dictionary<PeerHash, Bitfield> peers;
        private readonly ResourceBitfieldBookCollections books;

        public ResourceBitfield(ResourceStorageConfiguration configuration)
        {
            this.configuration = configuration;
            this.completed = new ResourceBitfieldMap(configuration);
            this.peers = new Dictionary<PeerHash, Bitfield>();
            this.books = new ResourceBitfieldBookCollections();
        }

        public ResourceBitfield(ResourceBitfield bitfield, ResourceStorageConfiguration configuration)
        {
            this.configuration = configuration;
            this.completed = new ResourceBitfieldMap(configuration);
            this.peers = bitfield.peers;
            this.books = bitfield.books;
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            peers.Add(peer, bitfield);
        }

        public int Count(int piece)
        {
            int count = 0;

            foreach (Bitfield bitfield in peers.Values)
            {
                if (bitfield[piece])
                {
                    count++;
                }
            }

            return count;
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

        public bool Complete(ResourceBlock block)
        {
            books.Complete(block);

            return completed.Complete(block.Index, block.Offset / configuration.BlockSize);
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

        public ResourceBlock[] Next(PeerHash peer, int maximum)
        {
            Bitfield bitfield;
            List<ResourceBlock> requests = new List<ResourceBlock>();

            long size = configuration.TotalSize;
            int left = Math.Min(maximum, maximum - books.Count(peer));

            if (peers.TryGetValue(peer, out bitfield))
            {
                for (int i = 0; left > 0 && i < configuration.Pieces; i++)
                {
                    if (bitfield[i] && completed.IsComplete(i) == false)
                    {
                        for (int j = 0; left > 0 && size > 0 && j < configuration.BlocksInPiece; j++)
                        {
                            if (completed.IsComplete(i, j) == false)
                            {
                                int offset = j * configuration.BlockSize;
                                int blockSize = configuration.BlockSize;

                                if (size < blockSize)
                                {
                                    blockSize = (int)size;
                                }

                                ResourceBlock block = new ResourceBlock(i, offset, blockSize);

                                if (books.Contains(block) == false && books.Contains(block, peer) == false)
                                {
                                    requests.Add(block);
                                    left--;
                                }
                            }

                            size = size - configuration.BlockSize;
                        }
                    }
                    else
                    {
                        size = size - configuration.BlocksInPiece * configuration.BlockSize;
                    }
                }
            }

            return requests.ToArray();
        }

        public void Reserve(PeerHash peer, ResourceBlock request)
        {
            books.Add(peer, request);
        }
    }
}