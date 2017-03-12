using System.Collections.Generic;
using Leak.Common;

namespace Leak.Data.Map.Components
{
    public class OmnibusReservationComparer : IEqualityComparer<BlockIndex>, IEqualityComparer<PieceInfo>
    {
        int IEqualityComparer<BlockIndex>.GetHashCode(BlockIndex obj)
        {
            return BlockIndex.GetHashCode(obj);
        }

        bool IEqualityComparer<BlockIndex>.Equals(BlockIndex x, BlockIndex y)
        {
            return BlockIndex.Equals(x, y);
        }

        int IEqualityComparer<PieceInfo>.GetHashCode(PieceInfo obj)
        {
            return PieceInfo.GetHashCode(obj);
        }

        bool IEqualityComparer<PieceInfo>.Equals(PieceInfo x, PieceInfo y)
        {
            return PieceInfo.Equals(x, y);
        }
    }
}