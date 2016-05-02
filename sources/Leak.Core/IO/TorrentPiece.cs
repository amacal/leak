using System.Linq;

namespace Leak.Core.IO
{
    public class TorrentPiece
    {
        private readonly MetainfoPiece source;
        private readonly long offset;
        private readonly int size;

        public TorrentPiece(MetainfoPiece source, long offset, int size)
        {
            this.source = source;
            this.offset = offset;
            this.size = size;
        }

        public int Index
        {
            get { return source.Index; }
        }

        public long Offset
        {
            get { return offset; }
        }

        public int Size
        {
            get { return size; }
        }

        public byte[] Hash
        {
            get { return source.Hash; }
        }

        public TorrentBlockCollection Blocks
        {
            get { return new TorrentBlockCollection(this); }
        }

        public bool IsCompleted(TorrentRepository repository)
        {
            return Blocks.All(repository.IsCompleted);
        }

        public override int GetHashCode()
        {
            return this.Index;
        }

        public override bool Equals(object obj)
        {
            TorrentPiece other = obj as TorrentPiece;

            return other != null && other.Index == this.Index;
        }
    }
}