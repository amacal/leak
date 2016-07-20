using System;

namespace Leak.Core.Retriever
{
    public class ResourceBitfieldMap
    {
        private readonly ResourceBitfieldBlock[] items;

        public ResourceBitfieldMap(ResourceConfiguration configuration)
        {
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

        public bool Complete(int piece, int block)
        {
            items[piece] = items[piece].Complete(block);

            return items[piece].IsComplete();
        }

        public void Invalidate(int piece)
        {
            items[piece] = items[piece].Invalidate();
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