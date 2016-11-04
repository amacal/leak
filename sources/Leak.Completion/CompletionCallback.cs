using System.Threading;

namespace Leak.Completion
{
    public interface CompletionCallback
    {
        void Complete(int affected);

        unsafe void Fail(NativeOverlapped* overlapped);
    }
}