using System.Threading;

namespace Leak.Completion
{
    public interface CompletionCallback
    {
        unsafe void Complete(NativeOverlapped* overlapped, int affected);

        unsafe void Fail(NativeOverlapped* overlapped);
    }
}