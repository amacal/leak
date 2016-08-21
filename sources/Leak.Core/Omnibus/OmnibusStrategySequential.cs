using System;
using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    public class OmnibusStrategySequential : OmnibusStrategy
    {
        public override IEnumerable<OmnibusBlock> Next(OmnibusStrategyContext context, int count)
        {
            DateTime now = DateTime.Now;
            int left = Math.Min(count, count - context.Reservations.Count(context.Peer));

            long totalSize = context.Configuration.Metainfo.Properties.TotalSize;
            int blockSize = context.Configuration.Metainfo.Properties.BlockSize;

            int blocks = context.Configuration.GetBlocksInPiece();
            int pieces = context.Configuration.Metainfo.Properties.Pieces;

            for (int i = 0; left > 0 && i < pieces; i++)
            {
                if (context.Bitfield[i] && context.Pieces.IsComplete(i) == false)
                {
                    for (int j = 0; left > 0 && totalSize > 0 && j < blocks; j++)
                    {
                        if (context.Pieces.IsComplete(i, j) == false)
                        {
                            int offset = j * blockSize;
                            int nextSize = blockSize;

                            if (totalSize < nextSize)
                            {
                                nextSize = (int)totalSize;
                            }

                            OmnibusBlock block = new OmnibusBlock(i, offset, nextSize);
                            bool contains = context.Reservations.Contains(block, now) ||
                                            context.Reservations.Contains(block, context.Peer);

                            if (contains == false)
                            {
                                left = left - 1;
                                yield return block;
                            }
                        }

                        totalSize = totalSize - blockSize;
                    }
                }
                else
                {
                    totalSize = totalSize - blocks * blockSize;
                }
            }
        }
    }
}