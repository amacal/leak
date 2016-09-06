using System;
using System.Collections.Generic;
using Leak.Core.Common;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusBitfieldRanking
    {
        private readonly int size;
        private readonly int lowest;
        private readonly int highest;
        private readonly int[] availabilities;

        public OmnibusBitfieldRanking(OmnibusBitfieldRanking source)
        {
            size = source.size;
            lowest = source.lowest;
            highest = source.highest;
            availabilities = new int[size];

            Array.Copy(source.availabilities, availabilities, size);
        }

        public OmnibusBitfieldRanking(Bitfield[] bitfields, int size)
        {
            this.size = size;

            availabilities = new int[size];
            lowest = bitfields.Length + 1;

            for (int i = 0; i < size; i++)
            {
                foreach (Bitfield bitfield in bitfields)
                {
                    if (bitfield[i])
                    {
                        availabilities[i]++;
                    }
                }

                if (availabilities[i] > highest)
                {
                    highest = availabilities[i];
                }

                if (lowest > availabilities[i])
                {
                    lowest = availabilities[i];
                }
            }
        }

        public OmnibusBitfieldRanking Exclude(OmnibusPieceCollection completed)
        {
            OmnibusBitfieldRanking result = new OmnibusBitfieldRanking(this);

            for (int i = 0; i < size; i++)
            {
                if (completed.IsComplete(i))
                {
                    result.availabilities[i] = -1;
                }
            }

            return result;
        }

        public OmnibusBitfieldRanking Include(Bitfield other)
        {
            OmnibusBitfieldRanking result = new OmnibusBitfieldRanking(this);

            for (int i = 0; i < size; i++)
            {
                if (other[i] == false)
                {
                    availabilities[i] = -1;
                }
            }

            return result;
        }

        public IEnumerable<Bitfield> Order()
        {
            for (int i = lowest; i <= highest; i++)
            {
                Bitfield bitfield = new Bitfield(size);

                for (int j = 0; j < size; j++)
                {
                    if (availabilities[j] == i)
                    {
                        bitfield[j] = true;
                    }
                }

                yield return bitfield;
            }
        }

        public int this[int index]
        {
            get { return availabilities[index]; }
        }
    }
}