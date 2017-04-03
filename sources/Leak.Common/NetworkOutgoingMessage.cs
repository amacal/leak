namespace Leak.Common
{
    public interface NetworkOutgoingMessage
    {
        int Length { get; }

        void ToBytes(DataBlock block);

        void Release();
    }
}