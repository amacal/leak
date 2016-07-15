namespace Leak.Core.Network
{
    public interface NetworkOutgoingMessage
    {
        int Length { get; }

        byte[] ToBytes();
    }
}