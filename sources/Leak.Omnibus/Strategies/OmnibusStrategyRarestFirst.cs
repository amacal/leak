using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Data.Map.Components;

namespace Leak.Data.Map.Strategies
{
    public class OmnibusStrategyRarestFirst : OmnibusStrategy
    {
        public override void Next(ICollection<BlockIndex> blocks, OmnibusContext context, PeerHash peer, int count)
        {
            int current = context.Reservations.Count(peer);
            int left = Math.Min(count, count - current);

            if (left > 0)
            {
                DateTime now = DateTime.Now;
                Bitfield bitfield = context.Bitfields.ByPeer(peer);

                int inPiece = context.Metainfo.GetBlocksInPiece();

                OmnibusPieceCollection pieces = context.Pieces;
                OmnibusReservationCollection reservations = context.Reservations;

                foreach (Bitfield best in context.Bitfields.Ranking.Order(bitfield))
                {
                    long totalSize = context.Metainfo.Properties.TotalSize;
                    int blockSize = context.Metainfo.Properties.BlockSize;

                    if (best.Minimum > 0)
                    {
                        totalSize = totalSize - (long)best.Minimum * inPiece * blockSize;
                    }

                    for (int i = best.Minimum; left > 0 && i <= best.Maximum; i++)
                    {
                        if (best[i])
                        {
                            for (int j = 0; left > 0 && totalSize > 0 && j < inPiece; j++)
                            {
                                if (pieces.IsComplete(i, j) == false)
                                {
                                    int offset = j * blockSize;
                                    int nextSize = blockSize;

                                    if (totalSize < nextSize)
                                    {
                                        nextSize = (int)totalSize;
                                    }

                                    BlockIndex block = new BlockIndex(i, offset, nextSize);
                                    bool contains = reservations.Contains(block, now) ||
                                                    reservations.Contains(block, peer);

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
    }
}