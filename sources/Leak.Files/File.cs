using System;

namespace Leak.Files
{
    public interface File : IDisposable
    {
        bool SetLength(long length);

        void Read(long position, FileBuffer buffer, FileReadCallback callback);

        void Write(long position, FileBuffer buffer, FileWriteCallback callback);

        bool Flush();
    }
}