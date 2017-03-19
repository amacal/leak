namespace Leak.Data.Store
{
    public interface RepositoryMemoryBlock
    {
        byte[] Data { get; }

        int Length { get; }

        void Release();
    }
}