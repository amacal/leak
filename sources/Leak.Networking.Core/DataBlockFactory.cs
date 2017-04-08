namespace Leak.Networking.Core
{
    public interface DataBlockFactory
    {
        DataBlock Transcient(byte[] data, int offset, int count);

        DataBlock Pooled(int size, DataBlockCallback callback);
    }
}