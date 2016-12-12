using Leak.Completion;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Files
{
    internal abstract class FileResult : IAsyncResult, CompletionCallback
    {
        public GCHandle? Pinned { get; set; }

        public IntPtr Handle { get; set; }

        public ManualResetEvent Event { get; set; }

        public bool IsCompleted { get; set; }

        public FileStatus Status { get; set; }

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

        public unsafe void Complete(NativeOverlapped* overlapped, int affected)
        {
            Affected = affected;
            IsCompleted = true;

            Event?.Set();
            Event?.Dispose();
            Pinned?.Free();

            Complete();
        }

        unsafe void CompletionCallback.Fail(NativeOverlapped* overlapped)
        {
            Fail();
        }

        public void Fail()
        {
            Fail(FileInterop.GetLastError());
        }

        public void Fail(uint code)
        {
            Status = (FileStatus)code;
            IsCompleted = true;

            Event?.Set();
            Event?.Dispose();
            Pinned?.Free();

            Complete();
        }

        protected abstract void Complete();
    }
}