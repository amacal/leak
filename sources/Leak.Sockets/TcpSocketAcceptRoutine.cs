using Leak.Completion;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal class TcpSocketAcceptRoutine
    {
        private readonly IntPtr handle;
        private readonly CompletionWorker worker;

        public TcpSocketAcceptRoutine(IntPtr handle, CompletionWorker worker)
        {
            this.handle = handle;
            this.worker = worker;
        }

        public unsafe void Execute(TcpSocketAcceptResult target)
        {
            TcpSocketInterop.AcceptExDelegate accept;
            if (GetDelegate(target, out accept, "{0xb5367df1,0xcbac,0x11cf,{0x95,0xca,0x00,0x80,0x5f,0x48,0xa1,0x92}}")) return;

            TcpSocketInterop.GetAcceptExSockaddrsDelegate sockaddr;
            if (GetDelegate(target, out sockaddr, "{0xb5367df2,0xcbac,0x11cf,{0x95,0xca,0x00,0x80,0x5f,0x48,0xa1,0x92}}")) return;

            byte[] data = new byte[64];
            IntPtr connection = TcpSocketInterop.WSASocket(2, 1, 6, IntPtr.Zero, 0, 1);
            target.Connection = new TcpSocketInstance(connection, worker);

            worker.Add(connection);
            target.Pin(data);

            IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            Overlapped overlapped = new Overlapped { AsyncResult = target };
            NativeOverlapped* native = overlapped.UnsafePack(null, null);

            target.Buffer = buffer;
            target.Sockaddrs = sockaddr;

            int received;
            int result = accept(handle, connection, buffer, 0, 32, 32, out received, native);
            uint error = TcpSocketInterop.GetLastError();

            if (result == 0 && error == 0)
            {
                if (received > 0)
                {
                    target.Complete(native, received);
                }
            }
            else if (result == 0 && error != 997)
            {
                target.Fail(error);
            }
        }

        private unsafe bool GetDelegate<T>(TcpSocketAcceptResult target, out T accept, string id)
        {
            int sent;
            IntPtr ptr = IntPtr.Zero;
            Guid guid = new Guid(id);
            int result = TcpSocketInterop.WSAIoctl(handle, unchecked((int)0xC8000006), ref guid, sizeof(Guid), out ptr, sizeof(IntPtr), out sent, IntPtr.Zero, IntPtr.Zero);

            if (result != 0)
            {
                target.Fail();
                accept = default(T);
                return true;
            }

            accept = (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
            return false;
        }
    }
}