using Leak.Common;

namespace Leak.Data.Share
{
    public interface DataShareToDataStore
    {
        void Read(BlockIndex index);
    }
}