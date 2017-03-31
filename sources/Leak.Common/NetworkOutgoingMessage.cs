namespace Leak.Common
{
    public interface NetworkOutgoingMessage
    {
        int Length { get; }

        DataBlock ToBytes(DataBlockFactory factory);
    }
}