using System;

namespace Leak.Common
{
    public interface DataBlock : IDisposable
    {
        int Size { get; }

        byte this[int index] { get; }

        void Write(DataBlockCallback callback);

        DataBlock Scope(int shift);
    }
}