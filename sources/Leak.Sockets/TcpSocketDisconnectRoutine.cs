using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class TcpSocketDisconnectRoutine
    {
        private readonly IntPtr handle;

        public TcpSocketDisconnectRoutine(IntPtr handle)
        {
            this.handle = handle;
        }

        public unsafe void Execute(TcpSocketDisconnectResult target)
        {
            int sent;
            IntPtr ptr = IntPtr.Zero;
            Guid guid = new Guid("{0x7fda2e11,0x8630,0x436f,{0xa0, 0x31, 0xf5, 0x36, 0xa6, 0xee, 0xc1, 0x57}} ");
            int result = TcpSocketInterop.WSAIoctl(handle, unchecked((int)0xC8000006), ref guid, sizeof(Guid), out ptr, sizeof(IntPtr), out sent, IntPtr.Zero, IntPtr.Zero);

            if (result != 0)
                throw new Exception();

            TcpSocketInterop.DisconnectExDelegate disconnect = (TcpSocketInterop.DisconnectExDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(TcpSocketInterop.DisconnectExDelegate));
            Overlapped overlapped = new Overlapped { AsyncResult = target };
            NativeOverlapped* native = overlapped.UnsafePack(null, null);

            result = disconnect.Invoke(handle, native, 0x02, 0);
            uint error = TcpSocketInterop.GetLastError();

            if (result == 0 && error != 997)
            {
                target.Fail(error);
            }
        }
    }
}