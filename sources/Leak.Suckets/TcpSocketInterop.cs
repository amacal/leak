using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Suckets
{
    internal class TcpSocketInterop
    {
        public unsafe delegate int ConnectExDelegate(
            IntPtr socket,
            IntPtr socketAddress,
            int socketAddressSize,
            IntPtr data,
            int dataLength,
            out int bytesSent,
            NativeOverlapped* overlapped);

        public unsafe delegate int AcceptExDelegate(
            IntPtr sListenSocket,
            IntPtr sAcceptSocket,
            IntPtr lpOutputBuffer,
            uint dwReceiveDataLength,
            uint dwLocalAddressLength,
            uint dwRemoteAddressLength,
            out int lpdwBytesReceived,
            NativeOverlapped* lpOverlapped);

        internal delegate void GetAcceptExSockaddrsDelegate(
                IntPtr buffer,
                int receiveDataLength,
                int localAddressLength,
                int remoteAddressLength,
                out IntPtr localSocketAddress,
                out int localSocketAddressLength,
                out IntPtr remoteSocketAddress,
                out int remoteSocketAddressLength);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern IntPtr WSASocket(
            [In] int addressFamily,
            [In] int socketType,
            [In] int protocolType,
            [In] IntPtr protocolInfo,
            [In] uint group,
            [In] uint flags);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern int closesocket(
            [In] IntPtr socket);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern int bind(
            [In] IntPtr socket,
            [In] IntPtr socketAddress,
            [In] int socketAddressSize);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern int listen(
            [In] IntPtr socket,
            [In] int backlog);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern int getsockname(
            [In] IntPtr socket,
            [In] IntPtr socketAddress,
            [In, Out] ref int socketAddressSize);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern int WSAIoctl(
            [In] IntPtr socket,
            [In] int ioControlCode,
            [In, Out] ref Guid guid,
            [In] int guidSize,
            [Out] out IntPtr funcPtr,
            [In]  int funcPtrSize,
            [Out] out int bytesTransferred,
            [In] IntPtr shouldBeNull,
            [In] IntPtr shouldBeNull2);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe uint WriteFile(
            [In] IntPtr hFile,
            [Out] IntPtr lpBuffer,
            [In] int numberOfBytesToWrite,
            [Out] out int umberOfBytesWritten,
            [In] NativeOverlapped* lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe uint ReadFile(
            [In] IntPtr hFile,
            [Out] IntPtr lpBuffer,
            [In] uint maxBytesToRead,
            [Out] out uint bytesActuallyRead,
            [In] NativeOverlapped* lpOverlapped);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern unsafe uint WSAGetOverlappedResult(
            [In] IntPtr handle,
            [In] NativeOverlapped* lpOverlapped,
            [Out] out uint ptrBytesTransferred,
            [In] bool wait,
            [Out] out uint flags);

        public static uint GetLastError()
        {
            return (uint)Marshal.GetLastWin32Error();
        }
    }
}