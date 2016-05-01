using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class TorrentPieceCollection : IEnumerable<TorrentPiece>
    {
        private readonly MetainfoFile metainfo;

        public TorrentPieceCollection(MetainfoFile metainfo)
        {
            this.metainfo = metainfo;
        }

        public int Size
        {
            get { return metainfo.Pieces.Size; }
        }

        public int Count
        {
            get { return metainfo.Pieces.Count; }
        }

        public TorrentPieceView GetPending(TorrentRepository repository)
        {
            return new TorrentPieceView(this, x => x.IsCompleted(repository) == false);
        }

        public TorrentPieceView GetCompleted(TorrentRepository repository)
        {
            return new TorrentPieceView(this, x => x.IsCompleted(repository) == true);
        }

        public IEnumerator<TorrentPiece> GetEnumerator()
        {
            long total = 0;
            long offset = 0;

            int size = metainfo.Pieces.Size;
            int count = metainfo.Pieces.Count;

            foreach (MetainfoEntry entry in metainfo.Entries)
            {
                total += entry.Size;
            }

            foreach (MetainfoPiece piece in metainfo.Pieces)
            {
                yield return new TorrentPiece(piece, offset, GetPieceSize(piece, total, size, count));
                offset += GetPieceSize(piece, total, size, count);
            }
        }

        private int GetPieceSize(MetainfoPiece piece, long total, int size, int count)
        {
            if (count - 1 > piece.Index)
            {
                return size;
            }

            if (total % size == 0)
            {
                return size;
            }

            return (int)(total % size);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}