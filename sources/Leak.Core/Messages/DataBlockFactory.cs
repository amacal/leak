using System;

namespace Leak.Core.Messages
{
    public interface DataBlockFactory
    {
        DataBlock Create(byte[] data, int offset, int count);

        DataBlock New(int count, Action<byte[], int, int> callback);
    }
}