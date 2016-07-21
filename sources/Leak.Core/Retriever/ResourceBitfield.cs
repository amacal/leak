using Leak.Core.Common;
using Leak.Core.Messages;
using System;
using System.Collections.Generic;

namespace Leak.Core.Retriever
{
    public class ResourceBitfield
    {
        private readonly ResourceConfiguration configuration;
        private readonly ResourceBitfieldMap completed;
        private readonly Dictionary<PeerHash, Bitfield> peers;
        private readonly ResourceBitfieldBookCollections books;

        public ResourceBitfield(ResourceConfiguration configuration)
        {
            this.configuration = configuration;
            this.completed = new ResourceBitfieldMap(configuration);
            this.peers = new Dictionary<PeerHash, Bitfield>();
            this.books = new ResourceBitfieldBookCollections();
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
            for (int i = 0; i < bitfield.Length; i++)
            {
                if (bitfield[i])
                {
                    completed.Complete(i);
                }
            }
        }

        public bool Complete(ResourcePieceRequest request)
        {
            books.Complete(request);

            return completed.Complete(request.Index, request.Offset / configuration.BlockSize);
        }

        public void Invalidate(int piece)
        {
            completed.Invalidate(piece);
        }

        public bool IsComplete(int piece)
        {
            return completed.IsComplete(piece);
        }

        public ResourcePieceRequest[] Next(PeerHash peer, int maximum)
        {
            Bitfield bitfield = peers[peer];
            List<ResourcePieceRequest> requests = new List<ResourcePieceRequest>();

            long size = configuration.TotalSize;
            int left = Math.Min(maximum, maximum - books.Count(peer));

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

                            ResourcePieceRequest request = new ResourcePieceRequest(i, offset, blockSize);

                            if (books.Contains(request) == false)
                            {
                                requests.Add(request);
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

            return requests.ToArray();
        }

        public void Book(PeerHash peer, ResourcePieceRequest request)
        {
            books.Add(peer, request);
        }
    }
}