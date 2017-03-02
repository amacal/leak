using System;

namespace Leak.Completion
{
    public interface CompletionWorker
    {
        void Add(IntPtr handle);
    }
}