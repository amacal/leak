using System;

namespace Leak.Core.Omnibus
{
    public class OmnibusPieceCollection
    {
        private OmnibusPiece[] items;
        private int completed;

        public OmnibusPieceCollection(OmnibusContext context)
        {
            int size = context.Metainfo.Pieces.Length;

            this.completed = 0;
            this.items = new OmnibusPiece[size];

            int left = context.Metainfo.GetBlocksInTotal();
            int blocks = context.Metainfo.GetBlocksInPiece();

            for (int i = 0; i < size; i++)
            {
                blocks = Math.Min(blocks, left);
                left = left - blocks;

                if (context.Bitfield[i])
                {
                    items[i] = new OmnibusPieceCompleted(blocks);
                    completed++;
                }
                else
                {
                    items[i] = new OmnibusPieceNothing(blocks);
                }
            }
        }

        public bool Complete(int piece, int block)
        {
            bool before = items[piece].IsComplete();

            items[piece] = items[piece].Complete(block);
            bool after = items[piece].IsComplete();

            if (before == false && after == true)
            {
                completed++;
            }

            return after;
        }

        public void Invalidate(int piece)
        {
            bool before = items[piece].IsComplete();

            items[piece] = items[piece].Invalidate();
            bool after = items[piece].IsComplete();

            if (before == true && after == false)
            {
                completed--;
            }
        }

        public bool IsComplete()
        {
            return items.Length == completed;
        }

        public bool IsComplete(int piece)
        {
            return items[piece].IsComplete();
        }

        public bool IsComplete(int piece, int block)
        {
            return items[piece].IsComplete(block);
        }
    }
}