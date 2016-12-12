using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Leak.Files
{
    internal class FileReadRoutine
    {
        private readonly IntPtr handle;
        private readonly FileBuffer buffer;

        public FileReadRoutine(IntPtr handle, FileBuffer buffer)
        {
            this.handle = handle;
            this.buffer = buffer;
        }

        public unsafe void Execute(FileReadResult target)
        {
            target.Pin(buffer.Data);

            Overlapped overlapped = new Overlapped
            {
                AsyncResult = target,
                OffsetLow = (int)(target.Position & 0xffffffff),
                OffsetHigh = (int)((target.Position >> 32) & 0xffffffff)
            };

            NativeOverlapped* native = overlapped.UnsafePack(null, null);
            IntPtr array = Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Data, buffer.Offset);

            int read;
            uint result = FileInterop.ReadFile(handle, array, buffer.Count, out read, native);
            uint error = FileInterop.GetLastError();

            if (result == 0 && error != 997)
            {
                target.Fail(error);
            }

            if (result == 1)
            {
                target.Complete(native, read);
            }
        }
    }
}