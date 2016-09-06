using System;
using System.Collections.Generic;
using Leak.Core.Common;
using Leak.Core.Omnibus.Components;

namespace Leak.Core.Omnibus.Strategies
{
    public class OmnibusStrategySequential : OmnibusStrategy
    {
        public override IEnumerable<OmnibusBlock> Next(OmnibusContext context, PeerHash peer, int count)
        {
            DateTime now = DateTime.Now;

            int left = Math.Min(count, count - context.Reservations.Count(peer));
            Bitfield bitfield = context.Bitfields.ByPeer(peer);

            long totalSize = context.Metainfo.Properties.TotalSize;
            int blockSize = context.Metainfo.Properties.BlockSize;

            int blocks = context.Metainfo.GetBlocksInPiece();
            int pieces = context.Metainfo.Properties.Pieces;

            for (int i = 0; left > 0 && i < pieces; i++)
            {
                if (bitfield[i] && context.Pieces.IsComplete(i) == false)
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
                                            context.Reservations.Contains(block, peer);

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