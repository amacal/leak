namespace Leak.Files
{
    internal class FileReadResult : FileResult
    {
        public File File { get; set; }

        public long Position { get; set; }

        public FileBuffer Buffer { get; set; }

        public FileReadCallback OnRead { get; set; }

        protected override void Complete()
        {
            OnRead?.Invoke(new FileRead(Status, Affected, Position, File, Buffer));
        }
    }
}