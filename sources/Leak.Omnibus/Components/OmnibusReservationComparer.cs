using System.Collections.Generic;
using Leak.Common;

namespace Leak.Datamap.Components
{
    public class OmnibusReservationComparer : IEqualityComparer<BlockIndex>
    {
        public int GetHashCode(BlockIndex obj)
        {
            return BlockIndex.GetHashCode(obj);
        }

        public bool Equals(BlockIndex x, BlockIndex y)
        {
            return BlockIndex.Equals(x, y);
        }
    }
}