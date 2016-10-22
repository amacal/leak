namespace Leak.Files
{
    public class FileRead
    {
        private readonly FileStatus status;
        private readonly int count;
        private readonly long position;
        private readonly File file;
        private readonly FileBuffer buffer;

        public FileRead(FileStatus status, int count, long position, File file, FileBuffer buffer)
        {
            this.status = status;
            this.count = count;
            this.position = position;
            this.file = file;
            this.buffer = buffer;
        }

        public FileStatus Status
        {
            get { return status; }
        }

        public int Count
        {
            get { return count; }
        }

        public File File
        {
            get { return file; }
        }

        public FileBuffer Buffer
        {
            get { return buffer; }
        }

        public long Position
        {
            get { return position; }
        }
    }
}