namespace Leak.Files
{
    internal class FileWriteResult : FileResult
    {
        public File File { get; set; }

        public long Position { get; set; }

        public FileBuffer Buffer { get; set; }

        public FileWriteCallback OnWritten { get; set; }

        protected override void Complete()
        {
            OnWritten.Invoke(new FileWrite(Status, Affected, Position, File, Buffer));
        }
    }
}