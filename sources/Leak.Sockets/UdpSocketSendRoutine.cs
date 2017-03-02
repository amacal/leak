using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class UdpSocketSendRoutine
    {
        private readonly IntPtr handle;
        private readonly IPEndPoint endpoint;
        private readonly SocketBuffer buffer;

        public UdpSocketSendRoutine(IntPtr handle, IPEndPoint endpoint, SocketBuffer buffer)
        {
            this.handle = handle;
            this.endpoint = endpoint;
            this.buffer = buffer;
        }

        public unsafe void Execute(UdpSocketSendResult target)
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

            byte[] address = endpoint.Address.GetAddressBytes();
            byte[] addressData = { 0x02, 0x00, (byte)(endpoint.Port / 256), (byte)(endpoint.Port % 256), address[0], address[1], address[2], address[3], 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            IntPtr addressBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(addressData, 0);

            target.Pin(addressData);

            int written;
            uint result = UdpSocketInterop.WSASendTo(handle, &data, 1, out written, 0, addressBuffer, addressData.Length, native, IntPtr.Zero);
            uint error = TcpSocketInterop.WSAGetLastError();

            if (result != 0 && error != 997)
            {
                target.Fail(error);
            }
        }
    }
}