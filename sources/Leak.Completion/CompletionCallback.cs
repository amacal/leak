using System.Threading;

namespace Leak.Suckets
{
    public interface CompletionCallback
    {
        void Complete(int affected);

        unsafe void Fail(NativeOverlapped* overlapped);
    }
}