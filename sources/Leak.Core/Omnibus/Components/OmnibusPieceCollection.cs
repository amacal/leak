using Leak.Core.Common;
using System;

namespace Leak.Core.Omnibus.Components
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
        }

        public void Complete(int piece, int block)
        {
            bool wasReady = items[piece].IsReady();
            items[piece] = items[piece].Complete(block);
            bool isReady = items[piece].IsReady();

            if (wasReady == false && isReady == true)
            {
                ready++;
                CallPieceReady(piece);
            }
        }

        public void Complete(int piece)
        {
            bool wasCompleted = items[piece].IsComplete();
            items[piece] = items[piece].Complete();
            bool isCompleted = items[piece].IsComplete();

            if (wasCompleted == false && isCompleted == true)
            {
                completed++;
                CallChanged();
                CallPieceCompleted(piece);
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
            FileHash hash = context.Metainfo.Hash;
            BitfieldInfo bitfield = new BitfieldInfo(items.Length, completed);

            context.Callback.OnProgressChanged(hash, bitfield);
        }

        private void CallPieceReady(int piece)
        {
            context.Callback.OnPieceReady(context.Metainfo.Hash, new PieceInfo(piece));
        }

        private void CallPieceCompleted(int piece)
        {
            context.Callback.OnPieceCompleted(context.Metainfo.Hash, new PieceInfo(piece));
        }

        private void CallFileCompleted()
        {
            if (items.Length == completed)
            {
                context.Callback.OnFileCompleted(context.Metainfo.Hash);
            }
        }
    }
}