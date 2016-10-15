using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusBitfieldRanking
    {
        private readonly OmnibusBitfieldCache cache;
        private readonly int size;
        private readonly int lowest;
        private readonly int highest;
        private readonly int[] availabilities;

        public OmnibusBitfieldRanking(OmnibusBitfieldRanking source, int[] location)
        {
            cache = source.cache;
            size = source.size;

            lowest = source.lowest;
            highest = source.highest;
            availabilities = location;

            Array.Copy(source.availabilities, availabilities, size);
        }

        public OmnibusBitfieldRanking(OmnibusBitfieldCache cache, Bitfield[] bitfields)
        {
            this.cache = cache;
            this.size = cache.Size;

            availabilities = cache.Ranking;
            lowest = bitfields.Length + 1;

            Array.Clear(availabilities, 0, size);

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
            OmnibusBitfieldRanking result = new OmnibusBitfieldRanking(this, cache.Excluded);

            for (int i = 0; i < size; i++)
            {
                if (result.availabilities[i] >= 0 && completed.IsComplete(i))
                {
                    result.availabilities[i] = -1;
                }
            }

            return result;
        }

        public OmnibusBitfieldRanking Include(Bitfield other)
        {
            OmnibusBitfieldRanking result = new OmnibusBitfieldRanking(this, cache.Included);

            for (int i = 0; i < size; i++)
            {
                if (other[i] == false)
                {
                    result.availabilities[i] = -1;
                }
            }

            return result;
        }

        public IEnumerable<Bitfield> Order()
        {
            Bitfield bitfield = cache.Bitfield;

            for (int i = lowest; i <= highest; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    bitfield[j] = availabilities[j] == i;
                }

                if (bitfield.Completed > 0)
                {
                    yield return bitfield;
                }
            }
        }
    }
}