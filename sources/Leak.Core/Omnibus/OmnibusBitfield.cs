using Leak.Core.Common;
using Leak.Core.Messages;
using System;
using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    public class OmnibusBitfield
    {
        private readonly OmnibusConfiguration configuration;
        private readonly OmnibusMap completed;
        private readonly Dictionary<PeerHash, Bitfield> peers;
        private readonly OmnibusReservationCollections books;

        public OmnibusBitfield(OmnibusConfiguration configuration)
        {
            this.configuration = configuration;
            this.completed = new OmnibusMap(configuration);
            this.peers = new Dictionary<PeerHash, Bitfield>();
            this.books = new OmnibusReservationCollections();
        }

        public OmnibusBitfield(OmnibusBitfield bitfield, OmnibusConfiguration configuration)
        {
            this.configuration = configuration;
            this.completed = new OmnibusMap(configuration);
            this.peers = bitfield.peers;
            this.books = bitfield.books;
        }

        public void Add(PeerHash peer, Bitfield bitfield)
        {
            peers[peer] = bitfield;
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

        public bool Complete(OmnibusBlock block)
        {
            books.Complete(block);

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
            Bitfield bitfield;
            peers.TryGetValue(peer, out bitfield);

            return bitfield?.IsCompleted() == true;
        }

        public OmnibusBlock[] Next(PeerHash peer, int maximum)
        {
            Bitfield bitfield;
            List<OmnibusBlock> requests = new List<OmnibusBlock>();

            long size = configuration.TotalSize;
            int left = Math.Min(maximum, maximum - books.Count(peer));

            int blocks = configuration.GetBlocksInPiece();
            DateTime now = DateTime.Now;

            if (peers.TryGetValue(peer, out bitfield))
            {
                for (int i = 0; left > 0 && i < configuration.Pieces; i++)
                {
                    if (bitfield[i] && completed.IsComplete(i) == false)
                    {
                        for (int j = 0; left > 0 && size > 0 && j < blocks; j++)
                        {
                            if (completed.IsComplete(i, j) == false)
                            {
                                int offset = j * configuration.BlockSize;
                                int blockSize = configuration.BlockSize;

                                if (size < blockSize)
                                {
                                    blockSize = (int)size;
                                }

                                OmnibusBlock block = new OmnibusBlock(i, offset, blockSize);

                                if (books.Contains(block, now) == false && books.Contains(block, peer) == false)
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
                        size = size - blocks * configuration.BlockSize;
                    }
                }
            }

            return requests.ToArray();
        }

        public PeerHash Reserve(PeerHash peer, OmnibusBlock request)
        {
            return books.Add(peer, request);
        }
    }
}