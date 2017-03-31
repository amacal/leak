using Leak.Common;

namespace Leak.Networking
{
    public interface NetworkPoolMemory
    {
        NetworkPoolMemoryBlock Allocate(int size);

        DataBlockFactory AsFactory();
    }
}