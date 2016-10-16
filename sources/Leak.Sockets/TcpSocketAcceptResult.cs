using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Leak.Sockets
{
    internal class TcpSocketAcceptResult : TcpSocketResult
    {
        public TcpSocket Socket { get; set; }

        public TcpSocket Connection { get; set; }

        public TcpSocketAcceptCallback OnAccepted { get; set; }

        public IntPtr Buffer { get; set; }

        public TcpSocketInterop.GetAcceptExSockaddrsDelegate Sockaddrs { get; set; }

        public TcpSocketAccept Unpack(IAsyncResult result)
        {
            return new TcpSocketAccept(Status, Socket, Connection, GetEndPoint);
        }

        protected override void OnCompleted(int affected)
        {
            OnAccepted?.Invoke(new TcpSocketAccept(Status, Socket, Connection, GetEndPoint));
        }

        protected override void OnFailed(TcpSocketStatus status)
        {
            OnAccepted?.Invoke(new TcpSocketAccept(Status, Socket, Connection, null));
        }

        private void GetEndPoint(out IPEndPoint local, out IPEndPoint remote)
        {
            IntPtr localAddr;
            int localAddrLength;
            IntPtr remoteAddr;
            int remoteAddrLength;

            Sockaddrs.Invoke(Buffer, Affected, 32, 32, out localAddr, out localAddrLength, out remoteAddr, out remoteAddrLength);

            byte[] localData = new byte[localAddrLength];
            byte[] remoteData = new byte[remoteAddrLength];

            Marshal.Copy(remoteAddr, remoteData, 0, remoteAddrLength);
            Marshal.Copy(localAddr, localData, 0, localAddrLength);

            byte[] localAddress = new byte[4];
            int localPort = 256 * localData[2] + localData[3];

            byte[] remoteAddress = new byte[4];
            int remotePort = 256 * remoteData[2] + remoteData[3];

            Array.Copy(localData, 4, localAddress, 0, 4);
            Array.Copy(remoteData, 4, remoteAddress, 0, 4);

            local = new IPEndPoint(new IPAddress(localAddress), localPort);
            remote = new IPEndPoint(new IPAddress(remoteAddress), remotePort);
        }
    }
}