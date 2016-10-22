using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Files
{
    internal static class FileInterop
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFile(
            [In] string fileName,
            [In] uint dwDesiredAccess,
            [In] uint dwShareMode,
            [In] IntPtr lpSecurityAttributes,
            [In] uint dwCreationDisposition,
            [In] uint dwFlagsAndAttributes,
            [In] IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(
            [In] IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetFilePointerEx(
            [In] IntPtr handle,
            [In] long distance,
            [Out] IntPtr pointer,
            [In] uint method);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetEndOfFile(
            [In] IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetFileValidData(
            [In] IntPtr handle,
            [In] long length);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe uint ReadFile(
            [In] IntPtr hFile,
            [Out] IntPtr lpBuffer,
            [In] int maxBytesToRead,
            [Out] out int bytesActuallyRead,
            [In] NativeOverlapped* lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe int WriteFile(
            [In] IntPtr hFile,
            [Out] IntPtr lpBuffer,
            [In] int numberOfBytesToWrite,
            [Out] out int umberOfBytesWritten,
            [In] NativeOverlapped* lpOverlapped);

        public static uint GetLastError()
        {
            return (uint)Marshal.GetLastWin32Error();
        }
    }
}