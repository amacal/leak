using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Sockets
{
    internal static class UdpSocketInterop
    {
        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern IntPtr WSASocket(
            [In] int addressFamily,
            [In] int socketType,
            [In] int protocolType,
            [In] IntPtr protocolInfo,
            [In] uint group,
            [In] uint flags);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern unsafe uint WSASendTo(
            [In] IntPtr socket,
            [In] TcpSocketInterop.WSABuffer* buffers,
            [In] int buffersCount,
            [Out] out int numberOfBytesSent,
            [In] int dwFlags,
            [In] IntPtr socketAddress,
            [In] int socketAddressSize,
            [In] NativeOverlapped* lpOverlapped,
            [In] IntPtr lpCompletionRoutine);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern unsafe int WSARecvFrom(
            [In] IntPtr socket,
            [In] TcpSocketInterop.WSABuffer* buffers,
            [In] int buffersCount,
            [Out] out int numberOfBytesReceived,
            [In, Out] ref int dwFlags,
            [In] IntPtr socketAddress,
            [In, Out] ref int socketAddressSize,
            [In] NativeOverlapped* lpOverlapped,
            [In] IntPtr lpCompletionRoutine);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern uint setsockopt(
            [In] IntPtr socket,
            [In] int optionLevel,
            [In] int optionName,
            [In] ref int optionValue,
            [In] int optionLength);

        [DllImport("ws2_32.dll", SetLastError = true)]
        public static extern int closesocket(
            [In] IntPtr socket);
    }
}