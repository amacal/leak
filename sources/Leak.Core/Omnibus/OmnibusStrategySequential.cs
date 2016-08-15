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

            long size = context.Configuration.TotalSize;
            int blocks = context.Configuration.GetBlocksInPiece();

            for (int i = 0; left > 0 && i < context.Configuration.Pieces; i++)
            {
                if (context.Bitfield[i] && context.Pieces.IsComplete(i) == false)
                {
                    for (int j = 0; left > 0 && size > 0 && j < blocks; j++)
                    {
                        if (context.Pieces.IsComplete(i, j) == false)
                        {
                            int offset = j * context.Configuration.BlockSize;
                            int blockSize = context.Configuration.BlockSize;

                            if (size < blockSize)
                            {
                                blockSize = (int)size;
                            }

                            OmnibusBlock block = new OmnibusBlock(i, offset, blockSize);
                            bool contains = context.Reservations.Contains(block, now) ||
                                            context.Reservations.Contains(block, context.Peer);

                            if (contains == false)
                            {
                                left = left - 1;
                                yield return block;
                            }
                        }

                        size = size - context.Configuration.BlockSize;
                    }
                }
                else
                {
                    size = size - blocks * context.Configuration.BlockSize;
                }
            }
        }
    }
}