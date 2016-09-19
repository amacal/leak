using System;

namespace Leak.Core.Messages
{
    public interface DataBlock : IDisposable
    {
        int Size { get; }

        byte this[int index] { get; }

        void Write(Action<byte[], int, int> stream);

        DataBlock Scope(int offset);
    }
}