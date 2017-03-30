namespace Leak.Data.Store
{
    public interface RepositoryMemory
    {
        RepositoryMemoryBlock Allocate(int size);

        void Release(byte[] data);
    }
}