using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class TcpSocketSendRoutine
    {
        private readonly IntPtr handle;
        private readonly SocketBuffer buffer;

        public TcpSocketSendRoutine(IntPtr handle, SocketBuffer buffer)
        {
            this.handle = handle;
            this.buffer = buffer;
        }

        public unsafe void Execute(TcpSocketSendResult target)
        {
            target.Pin(buffer.Data);

            Overlapped overlapped = new Overlapped { AsyncResult = target };
            NativeOverlapped* native = overlapped.UnsafePack(null, null);
            IntPtr reference = Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Data, buffer.Offset);

            TcpSocketInterop.WSABuffer data = new TcpSocketInterop.WSABuffer
            {
                length = buffer.Count,
                buffer = reference
            };

            int written;
            uint result = TcpSocketInterop.WSASend(handle, &data, 1, out written, 0, native, IntPtr.Zero);
            uint error = TcpSocketInterop.WSAGetLastError();

            if (result != 0 && error != 997)
            {
                target.Fail(error);
            }
        }
    }
}