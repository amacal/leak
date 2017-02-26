using Leak.Completion;
using System;
using System.Net.Sockets;

namespace Leak.Sockets
{
    public class SocketFactory
    {
        private readonly CompletionWorker worker;

        public SocketFactory(CompletionWorker worker)
        {
            using (new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                this.worker = worker;
            }
        }

        public TcpSocket Tcp()
        {
            IntPtr handle = TcpSocketInterop.WSASocket(2, 1, 6, IntPtr.Zero, 0, 1);

            if (handle == new IntPtr(-1))
                throw new Exception();

            return new TcpSocketInstance(handle, worker);
        }

        public UdpSocket Udp()
        {
            IntPtr handle = UdpSocketInterop.WSASocket(2, 2, 17, IntPtr.Zero, 0, 1);

            if (handle == new IntPtr(-1))
                throw new Exception();

            return new UdpSocketInstance(handle, worker);
        }
    }
}