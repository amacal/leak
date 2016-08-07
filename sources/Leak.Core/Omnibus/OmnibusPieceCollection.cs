using System;

namespace Leak.Core.Omnibus
{
    public class OmnibusPieceCollection
    {
        private OmnibusPiece[] items;
        private int completed;

        public OmnibusPieceCollection(OmnibusConfiguration configuration)
        {
            this.completed = 0;
            this.items = new OmnibusPiece[configuration.Pieces];

            int left = configuration.GetBlocksInTotal();
            int blocks = configuration.GetBlocksInPiece();

            for (int i = 0; i < configuration.Pieces; i++)
            {
                blocks = Math.Min(blocks, left);
                left = left - blocks;

                items[i] = new OmnibusPieceNothing(blocks);
            }
        }

        public void Reduce(int size)
        {
            Array.Resize(ref items, size);
        }

        public void Complete(int piece)
        {
            bool before = items[piece].IsComplete();

            items[piece] = items[piece].Complete();
            bool after = items[piece].IsComplete();

            if (before == false && after == true)
            {
                completed++;
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