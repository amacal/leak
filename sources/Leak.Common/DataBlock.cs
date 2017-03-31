namespace Leak.Common
{
    public interface DataBlock
    {
        int Length { get; }

        void With(DataBlockCallback callback);

        DataBlock Scope(int shift);

        void Release();
    }
}