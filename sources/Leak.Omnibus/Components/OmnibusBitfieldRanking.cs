using System;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Data.Map.Components
{
    public class OmnibusBitfieldRanking
    {
        private Bitfield buffer;
        private int[] available;
        private int highest;

        public OmnibusBitfieldRanking()
        {
            buffer = new Bitfield(0);
            available = new int[0];
            highest = 0;
        }

        public void Add(Bitfield bitfield)
        {
            ExpandIfNeeded(bitfield.Length);

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (available[i] >= 0)
                {
                    available[i]++;
                    highest = Math.Max(highest, available[i]);
                }
            }
        }

        public void Remove(Bitfield bitfield)
        {
            ExpandIfNeeded(bitfield.Length);

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (available[i] > 0)
                {
                    available[i]--;
                }
            }
        }

        public void Add(PieceInfo piece)
        {
            ExpandIfNeeded(piece.Index);

            if (available[piece.Index] >= 0)
            {
                available[piece.Index]++;
                highest = Math.Max(highest, available[piece.Index]);
            }
        }

        public void Complete(Bitfield bitfield)
        {
            ExpandIfNeeded(bitfield.Length);

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (bitfield[i])
                {
                    available[i] = -1;
                }
            }
        }

        public void Complete(PieceInfo piece)
        {
            ExpandIfNeeded(piece.Index);
            available[piece.Index] = -1;
        }

        public IEnumerable<Bitfield> Order(Bitfield other)
        {
            int size = Math.Min(buffer.Length, other.Length);

            for (int i = 0; i <= highest; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    buffer[j] = other[j] && available[j] == i;
                }

                if (buffer.Completed > 0)
                {
                    yield return buffer;
                }
            }
        }

        private void ExpandIfNeeded(int size)
        {
            if (available.Length < size)
            {
                buffer = new Bitfield(size);
                Array.Resize(ref available, size);
            }
        }
    }
}