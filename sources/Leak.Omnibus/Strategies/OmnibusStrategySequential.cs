using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Data.Map.Components;

namespace Leak.Data.Map.Strategies
{
    public class OmnibusStrategySequential : OmnibusStrategy
    {
        public override void Next(ICollection<BlockIndex> blocks, OmnibusContext context, PeerHash peer, int count)
        {
            DateTime now = DateTime.Now;

            int left = Math.Min(count, count - context.Reservations.Count(peer));
            Bitfield bitfield = context.Bitfields.ByPeer(peer);

            long totalSize = context.Metainfo.Properties.TotalSize;
            int blockSize = context.Metainfo.Properties.BlockSize;

            int inPiece = context.Metainfo.GetBlocksInPiece();
            int pieces = context.Metainfo.Properties.Pieces;

            for (int i = 0; left > 0 && i < pieces; i++)
            {
                if (bitfield[i] && context.Pieces.IsComplete(i) == false)
                {
                    for (int j = 0; left > 0 && totalSize > 0 && j < inPiece; j++)
                    {
                        if (context.Pieces.IsComplete(i, j) == false)
                        {
                            int offset = j * blockSize;
                            int nextSize = blockSize;

                            if (totalSize < nextSize)
                            {
                                nextSize = (int)totalSize;
                            }

                            BlockIndex block = new BlockIndex(i, offset, nextSize);
                            bool contains = context.Reservations.Contains(block, now) ||
                                            context.Reservations.Contains(block, peer);

                            if (contains == false)
                            {
                                left = left - 1;
                                blocks.Add(block);
                            }
                        }

                        totalSize = totalSize - blockSize;
                    }
                }
                else
                {
                    totalSize = totalSize - inPiece * blockSize;
                }
            }
        }
    }
}