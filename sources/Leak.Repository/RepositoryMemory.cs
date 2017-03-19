namespace Leak.Data.Store
{
    public interface RepositoryMemory
    {
        RepositoryMemoryBlock Allocate(int size);
    }
}