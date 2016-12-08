namespace Leak.Common
{
    public interface NetworkOutgoingMessage
    {
        int Length { get; }

        byte[] ToBytes();
    }
}