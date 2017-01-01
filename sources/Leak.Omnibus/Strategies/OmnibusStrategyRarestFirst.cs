using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Omnibus.Components;

namespace Leak.Omnibus.Strategies
{
    public class OmnibusStrategyRarestFirst : OmnibusStrategy
    {
        public override void Next(ICollection<BlockIndex> blocks, OmnibusContext context, PeerHash peer, int count)
        {
            int current = context.Reservations.Count(peer);
            int left = Math.Min(count, count - current);

            if (left > 0 && current <= context.Configuration.SchedulerThreshold)
            {
                DateTime now = DateTime.Now;
                Bitfield bitfield = context.Bitfields.ByPeer(peer);

                int inPiece = context.Metainfo.GetBlocksInPiece();
                int total = context.Metainfo.Properties.Pieces;

                OmnibusBitfieldRanking ranking = context.Bitfields.Ranking;
                OmnibusBitfieldRanking adjusted = ranking.Exclude(context.Pieces).Include(bitfield);

                OmnibusPieceCollection pieces = context.Pieces;
                OmnibusReservationCollection reservations = context.Reservations;

                foreach (Bitfield best in adjusted.Order())
                {
                    int positive = best.Completed;
                    long totalSize = context.Metainfo.Properties.TotalSize;
                    int blockSize = context.Metainfo.Properties.BlockSize;

                    for (int i = 0; left > 0 && i < total && positive > 0; i++)
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

                            positive = positive - 1;
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