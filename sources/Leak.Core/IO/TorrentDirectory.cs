namespace Leak.Core.IO
{
    public class TorrentDirectory
    {
        private readonly MetainfoFile metainfo;
        private readonly string path;

        public TorrentDirectory(MetainfoFile metainfo, string path)
        {
            this.metainfo = metainfo;
            this.path = path;
        }

        public string Path
        {
            get { return path; }
        }

        public TorrentFileCollection Files
        {
            get { return new TorrentFileCollection(metainfo, path); }
        }

        public TorrentPieceCollection Pieces
        {
            get { return new TorrentPieceCollection(metainfo); }
        }
    }
}