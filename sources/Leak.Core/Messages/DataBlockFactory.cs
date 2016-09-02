namespace Leak.Core.Messages
{
    public interface DataBlockFactory
    {
        DataBlock Create(byte[] data, int offset, int count);
    }
}