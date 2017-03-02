using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class UdpSocketReceiveRoutine
    {
        private readonly IntPtr handle;
        private readonly SocketBuffer buffer;

        public UdpSocketReceiveRoutine(IntPtr handle, SocketBuffer buffer)
        {
            this.handle = handle;
            this.buffer = buffer;
        }

        public unsafe void Execute(UdpSocketReceiveResult target)
        {
            target.Pin(buffer.Data);

            IntPtr array = Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Data, buffer.Offset);
            Overlapped overlapped = new Overlapped { AsyncResult = target };
            NativeOverlapped* native = overlapped.UnsafePack(null, null);

            TcpSocketInterop.WSABuffer data = new TcpSocketInterop.WSABuffer
            {
                length = buffer.Count,
                buffer = array
            };

            byte[] addressData = new byte[16];
            IntPtr addressPointer = Marshal.UnsafeAddrOfPinnedArrayElement(addressData, 0);

            target.Address = addressData;
            target.Pin(addressData);

            int read, flags = 0, size = addressData.Length;
            int result = UdpSocketInterop.WSARecvFrom(handle, &data, 1, out read, ref flags, addressPointer, ref size, native, IntPtr.Zero);
            uint error = TcpSocketInterop.GetLastError();

            if (result == -1 && error != 997)
            {
                target.Fail(error);
            }
        }
    }
}