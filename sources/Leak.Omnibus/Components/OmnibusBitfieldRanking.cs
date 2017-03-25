using System;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Data.Map.Components
{
    public class OmnibusBitfieldRanking
    {
        private Bitfield buffer;
        private int[] available;

        public OmnibusBitfieldRanking()
        {
            buffer = new Bitfield(0);
            available = new int[0];
        }

        public void Add(Bitfield bitfield)
        {
            ExpandIfNeeded(bitfield.Length);

            for (int i = 0; i < bitfield.Length; i++)
            {
                if (available[i] >= 0 && bitfield[i])
                {
                    available[i]++;
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
            int lowest = Int32.MaxValue;
            int highest = Int32.MinValue;
            int maximum = buffer.Length;

            if (other.Length < maximum)
            {
                maximum = other.Maximum;
            }

            for (int i = other.Minimum; i < maximum; i++)
            {
                if (other[i])
                {
                    if (lowest > available[i])
                    {
                        lowest = available[i];
                    }

                    if (highest < available[i])
                    {
                        highest = available[i];
                    }
                }
            }

            if (highest > 0)
            {
                for (int i = lowest; i <= highest; i++)
                {
                    buffer.Clear();

                    for (int j = other.Minimum; j <= other.Maximum; j++)
                    {
                        if (other[j] && available[j] == i)
                        {
                            buffer[j] = true;
                        }
                    }

                    if (buffer.Completed > 0)
                    {
                        yield return buffer;
                    }
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