using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class TcpSocketSendRoutine
    {
        private readonly IntPtr handle;
        private readonly TcpSocketBuffer buffer;

        public TcpSocketSendRoutine(IntPtr handle, TcpSocketBuffer buffer)
        {
            this.handle = handle;
            this.buffer = buffer;
        }

        public unsafe void Execute(TcpSocketSendResult target)
        {
            target.Pin(buffer.Data);

            Overlapped overlapped = new Overlapped { AsyncResult = target };
            NativeOverlapped* native = overlapped.UnsafePack(null, null);
            IntPtr reference = Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Data, 0);

            int written;
            uint result = TcpSocketInterop.WriteFile(handle, reference, buffer.Count, out written, native);
            uint error = TcpSocketInterop.GetLastError();

            if (result == 0 && error != 997)
            {
                target.Fail(error);
            }
        }
    }
}