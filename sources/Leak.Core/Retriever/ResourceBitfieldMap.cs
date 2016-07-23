using System;

namespace Leak.Core.Retriever
{
    public class ResourceBitfieldMap
    {
        private readonly ResourceBitfieldBlock[] items;
        private int completed;

        public ResourceBitfieldMap(ResourceStorageConfiguration configuration)
        {
            this.completed = 0;
            this.items = new ResourceBitfieldBlock[configuration.Pieces];

            int left = configuration.Blocks;
            int blocks = configuration.BlocksInPiece;

            for (int i = 0; i < configuration.Pieces; i++)
            {
                blocks = Math.Min(blocks, left);
                left = left - blocks;

                items[i] = new ResourceBitfieldBlockToNothing(blocks);
            }
        }

        public void Complete(int piece)
        {
            bool before = items[piece].IsComplete();

            items[piece] = items[piece].Complete();
            bool after = items[piece].IsComplete();

            if (before != after)
            {
                completed++;
            }
        }

        public bool Complete(int piece, int block)
        {
            bool before = items[piece].IsComplete();

            items[piece] = items[piece].Complete(block);
            bool after = items[piece].IsComplete();

            if (before != after)
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

            if (before != after)
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