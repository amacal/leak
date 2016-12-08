using Leak.Completion;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class TcpSocketConnectRoutine
    {
        private readonly IntPtr handle;
        private readonly CompletionWorker worker;
        private readonly IPEndPoint endpoint;

        public TcpSocketConnectRoutine(IntPtr handle, CompletionWorker worker, IPEndPoint endpoint)
        {
            this.handle = handle;
            this.worker = worker;
            this.endpoint = endpoint;
        }

        public unsafe void Execute(TcpSocketConnectResult target)
        {
            int sent;
            IntPtr ptr = IntPtr.Zero;
            Guid guid = new Guid("{0x25a207b9,0x0ddf3,0x4660,{0x8e,0xe9,0x76,0xe5,0x8c,0x74,0x06,0x3e}}");
            int result = TcpSocketInterop.WSAIoctl(handle, unchecked((int)0xC8000006), ref guid, sizeof(Guid), out ptr, sizeof(IntPtr), out sent, IntPtr.Zero, IntPtr.Zero);

            if (result != 0)
                throw new Exception();

            byte[] address = endpoint.Address.GetAddressBytes();
            byte[] data = { 0x02, 0x00, (byte)(endpoint.Port / 256), (byte)(endpoint.Port % 256), address[0], address[1], address[2], address[3], 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            TcpSocketInterop.ConnectExDelegate connectex = (TcpSocketInterop.ConnectExDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(TcpSocketInterop.ConnectExDelegate));

            Overlapped overlapped = new Overlapped { AsyncResult = target };
            NativeOverlapped* native = overlapped.UnsafePack(null, null);

            target.Pin(data);
            worker.Add(handle);

            IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);

            result = connectex.Invoke(handle, buffer, data.Length, IntPtr.Zero, 0, out sent, native);
            uint error = TcpSocketInterop.GetLastError();

            if (result == 0 && error != 997)
            {
                target.Fail(error);
            }
        }
    }
}