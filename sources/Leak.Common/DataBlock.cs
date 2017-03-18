namespace Leak.Common
{
    public interface DataBlock
    {
        int Size { get; }

        byte this[int index] { get; }

        void Write(DataBlockCallback callback);

        DataBlock Scope(int shift);

        void Release();
    }
}