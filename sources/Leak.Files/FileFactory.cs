using System;
using Leak.Completion;

namespace Leak.Files
{
    public class FileFactory
    {
        private readonly CompletionWorker worker;

        public FileFactory(CompletionWorker worker)
        {
            this.worker = worker;
        }

        public File Open(string path)
        {
            IntPtr handle = FileInterop.CreateFile(path, 0x80000000 | 0x40000000, 0x01 | 0x02, IntPtr.Zero, 0x03, 0x40000000 | 0x00000080, IntPtr.Zero);
            uint error = FileInterop.GetLastError();

            if (handle == new IntPtr(-1))
                return null;

            worker.Add(handle);

            return new FileInstance(handle, worker);
        }

        public File OpenOrCreate(string path)
        {
            IntPtr handle = FileInterop.CreateFile(path, 0x80000000 | 0x40000000, 0x01 | 0x02, IntPtr.Zero, 0x04, 0x40000000 | 0x00000080, IntPtr.Zero);
            uint error = FileInterop.GetLastError();

            worker.Add(handle);

            return new FileInstance(handle, worker);
        }
    }
}