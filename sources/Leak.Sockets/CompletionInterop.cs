using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Suckets
{
    internal class CompletionInterop
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateIoCompletionPort(
            [In] IntPtr handle,
            [In] IntPtr port,
            [In] uint key,
            [In] uint threads);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe bool GetQueuedCompletionStatus(
            [In] IntPtr completionPort,
            [Out] out uint ptrBytesTransferred,
            [Out] out uint ptrCompletionKey,
            [Out] NativeOverlapped** lpOverlapped,
            [In] uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe uint GetOverlappedResult(
            [In] IntPtr handle,
            [In] NativeOverlapped* lpOverlapped,
            [Out] out uint ptrBytesTransferred,
            [In] bool wait);
    }
}