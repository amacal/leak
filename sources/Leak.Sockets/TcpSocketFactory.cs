using System;
using System.Net.Sockets;
using Leak.Suckets;

namespace Leak.Sockets
{
    public class TcpSocketFactory
    {
        private readonly CompletionWorker worker;

        public TcpSocketFactory(CompletionWorker worker)
        {
            using (new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                this.worker = worker;
            }
        }

        public TcpSocket Create()
        {
            IntPtr handle = TcpSocketInterop.WSASocket(2, 1, 6, IntPtr.Zero, 0, 1);

            if (handle == new IntPtr(-1))
                throw new Exception();

            return new TcpSocketInstance(handle, worker);
        }
    }
}