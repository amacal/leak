using System;

namespace Leak.Suckets
{
    public interface CompletionWorker
    {
        void Add(IntPtr handle);

        void Remove(IntPtr handle);
    }
}