using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class TcpSocketReceiveRoutine
    {
        private readonly IntPtr handle;
        private readonly SocketBuffer buffer;

        public TcpSocketReceiveRoutine(IntPtr handle, SocketBuffer buffer)
        {
            this.handle = handle;
            this.buffer = buffer;
        }

        public unsafe void Execute(SocketResult target)
        {
            target.Pin(buffer.Data);

            IntPtr array = Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Data, buffer.Offset);
            Overlapped overlapped = new Overlapped { AsyncResult = target };
            NativeOverlapped* native = overlapped.UnsafePack(null, null);

            uint read;
            uint result = TcpSocketInterop.ReadFile(handle, array, (uint)buffer.Count, out read, native);
            uint error = TcpSocketInterop.GetLastError();

            if (result == 0 && error != 997)
            {
                target.Fail(error);
            }
        }
    }
}