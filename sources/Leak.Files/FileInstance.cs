using Leak.Completion;
using System;

namespace Leak.Files
{
    public class FileInstance : File
    {
        private readonly IntPtr handle;
        private readonly CompletionWorker worker;

        public FileInstance(IntPtr handle, CompletionWorker worker)
        {
            this.handle = handle;
            this.worker = worker;
        }

        public bool SetLength(long length)
        {
            return
                FileInterop.SetFilePointerEx(handle, length, IntPtr.Zero, 0x00) &&
                FileInterop.SetEndOfFile(handle);
        }

        public void Read(long position, FileBuffer buffer, FileReadCallback callback)
        {
            FileReadRoutine routine = new FileReadRoutine(handle, buffer);
            FileReadResult result = new FileReadResult
            {
                Handle = handle,
                File = this,
                Position = position,
                Buffer = buffer,
                OnRead = callback
            };

            routine.Execute(result);
        }

        public void Write(long position, FileBuffer buffer, FileWriteCallback callback)
        {
            FileWriteRoutine routine = new FileWriteRoutine(handle, buffer);
            FileWriteResult result = new FileWriteResult
            {
                Handle = handle,
                File = this,
                Position = position,
                Buffer = buffer,
                OnWritten = callback
            };

            routine.Execute(result);
        }

        public bool Flush()
        {
            return FileInterop.FlushFileBuffers(handle);
        }

        public void Dispose()
        {
            FileInterop.CloseHandle(handle);
        }
    }
}