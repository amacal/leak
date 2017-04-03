using Leak.Completion;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal abstract class SocketResult : IAsyncResult, CompletionCallback
    {
        public GCHandle? Pinned1 { get; set; }
        public GCHandle? Pinned2 { get; set; }

        public IntPtr Handle { get; set; }

        public ManualResetEvent Event { get; set; }

        public bool IsCompleted { get; set; }

        public SocketStatus Status { get; set; }

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
            if (Pinned1.HasValue == false)
            {
                Pinned1 = GCHandle.Alloc(instance, GCHandleType.Pinned);
            }
            else
            {
                Pinned2 = GCHandle.Alloc(instance, GCHandleType.Pinned);
            }
        }

        public unsafe void Complete(NativeOverlapped* overlapped, int affected)
        {
            uint ignore;
            uint flags;

            uint result = TcpSocketInterop.WSAGetOverlappedResult(Handle, overlapped, out ignore, false, out flags);

            Affected = affected;
            IsCompleted = true;

            if (result != 0 || affected > 0)
            {
                Release();
                OnCompleted();
            }
            else
            {
                Fail();
            }
        }

        public void Fail()
        {
            Fail(TcpSocketInterop.GetLastError());
        }

        public void Fail(uint reason)
        {
            Status = (SocketStatus)reason;

            Release();
            OnFailed();
        }

        private void Release()
        {
            Event?.Set();
            Event?.Dispose();

            Pinned1?.Free();
            Pinned2?.Free();
        }

        protected abstract void OnCompleted();

        protected abstract void OnFailed();
    }
}