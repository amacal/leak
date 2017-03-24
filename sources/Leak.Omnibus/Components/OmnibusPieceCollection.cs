using System;
using Leak.Common;

namespace Leak.Data.Map.Components
{
    public class OmnibusPieceCollection
    {
        private readonly OmnibusContext context;
        private readonly OmnibusPiece[] items;

        private int ready;
        private int completed;

        public OmnibusPieceCollection(OmnibusContext context)
        {
            this.context = context;
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
                    ready++;
                }
                else
                {
                    items[i] = new OmnibusPieceNothing(blocks);
                }
            }

            CallFileCompleted();
        }

        public void Complete(int piece, int block)
        {
            bool wasReady = items[piece].IsReady();
            items[piece] = items[piece].Complete(block);
            bool isReady = items[piece].IsReady();

            if (wasReady == false && isReady)
            {
                ready++;
            }

            if (isReady)
            {
                CallPieceReady(piece);
            }
        }

        public void Complete(PieceInfo piece)
        {
            bool wasCompleted = items[piece.Index].IsComplete();
            items[piece.Index] = items[piece.Index].Complete();
            bool isCompleted = items[piece.Index].IsComplete();

            if (wasCompleted == false && isCompleted == true)
            {
                completed++;
                CallChanged();
                CallPieceCompleted(piece.Index);
                CallFileCompleted();
            }
        }

        public void Invalidate(int piece)
        {
            bool wasCompleted = items[piece].IsComplete();
            items[piece] = items[piece].Invalidate();
            bool isCompleted = items[piece].IsComplete();

            if (wasCompleted == true && isCompleted == false)
            {
                completed--;
                CallChanged();
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

        private void CallChanged()
        {
            context.Hooks.CallDataChanged(context.Metainfo.Hash, completed);
        }

        private void CallPieceReady(int index)
        {
            context.Hooks.CallPieceReady(context.Metainfo.Hash, new PieceInfo(index));
        }

        private void CallPieceCompleted(int piece)
        {
            context.Bitfields.Ranking.Complete(new PieceInfo(piece));
            context.Hooks.CallPieceCompleted(context.Metainfo.Hash, piece);
        }

        private void CallFileCompleted()
        {
            if (items.Length == completed)
            {
                context.Hooks.CallDataCompleted(context.Metainfo.Hash);
            }
        }
    }
}