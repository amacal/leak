namespace Leak.Core.IO
{
    public class TorrentFile
    {
        private readonly MetainfoEntry source;
        private readonly string directory;
        private readonly long offset;

        public TorrentFile(MetainfoEntry source, string directory, long offset)
        {
            this.source = source;
            this.directory = directory;
            this.offset = offset;
        }

        public string Name
        {
            get { return source.Name; }
        }

        public string Path
        {
            get { return System.IO.Path.Combine(directory, source.Name); }
        }

        public long Offset
        {
            get { return offset; }
        }

        public long Size
        {
            get { return source.Size; }
        }
    }
}