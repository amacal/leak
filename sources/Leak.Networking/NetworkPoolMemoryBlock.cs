namespace Leak.Networking
{
    public interface NetworkPoolMemoryBlock
    {
        byte[] Data { get; }

        int Length { get; }

        void Release();
    }
}