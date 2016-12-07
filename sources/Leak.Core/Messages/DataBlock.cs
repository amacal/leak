using System;

namespace Leak.Core.Messages
{
    public interface DataBlock : IDisposable
    {
        int Size { get; }

        byte this[int index] { get; }

        void Write(DataBlockCallback callback);

        DataBlock Scope(int offset);
    }
}