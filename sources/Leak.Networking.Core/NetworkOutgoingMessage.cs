namespace Leak.Networking.Core
{
    public interface NetworkOutgoingMessage
    {
        int Length { get; }

        void ToBytes(DataBlock block);

        void Release();
    }
}