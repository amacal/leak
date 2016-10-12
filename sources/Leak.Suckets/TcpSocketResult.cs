using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Suckets
{
    internal abstract class TcpSocketResult : IAsyncResult, CompletionCallback
    {
        public GCHandle? Pinned { get; set; }

        public IntPtr Handle { get; set; }

        public ManualResetEvent Event { get; set; }

        public bool IsCompleted { get; set; }

        public TcpSocketStatus Status { get; set; }

        public int Affected { get; set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return Event; }
        }

        public object AsyncState
        {
            get { return null; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public void Pin(object instance)
        {
            Pinned = GCHandle.Alloc(instance, GCHandleType.Pinned);
        }

        public void Complete(int affected)
        {
            Affected = affected;
            IsCompleted = true;

            Event?.Set();
            Event?.Dispose();
            Pinned?.Free();

            OnCompleted(affected);
        }

        unsafe void CompletionCallback.Fail(NativeOverlapped* overlapped)
        {
            uint affected;
            uint flags;

            TcpSocketInterop.WSAGetOverlappedResult(Handle, overlapped, out affected, false, out flags);

            Fail();
        }

        public void Fail()
        {
            Fail(TcpSocketInterop.GetLastError());
        }

        public void Fail(uint code)
        {
            Status = (TcpSocketStatus)code;
            IsCompleted = true;

            Event?.Set();
            Event?.Dispose();
            Pinned?.Free();

            OnFailed(Status);
        }

        protected abstract void OnCompleted(int affected);

        protected abstract void OnFailed(TcpSocketStatus status);
    }
}