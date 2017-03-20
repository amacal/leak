using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Leak.Sockets
{
    internal class SocketBindRoutine
    {
        private readonly IPEndPoint endpoint;

        public SocketBindRoutine(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public bool Execute(IntPtr handle)
        {
            byte[] data = { 0x02, 0x00, (byte)(endpoint.Port / 256), (byte)(endpoint.Port % 256), 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            GCHandle pinned = GCHandle.Alloc(data, GCHandleType.Pinned);

            byte[] bytes = endpoint.Address.GetAddressBytes();
            Array.Copy(bytes, 0, data, 4, 4);

            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            int value = TcpSocketInterop.bind(handle, pointer, data.Length);

            pinned.Free();

            return value == 0;
        }
    }
}