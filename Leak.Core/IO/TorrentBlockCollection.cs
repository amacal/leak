using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class TorrentBlockCollection : IEnumerable<TorrentBlock>
    {
        private readonly TorrentPiece piece;

        public TorrentBlockCollection(TorrentPiece piece)
        {
            this.piece = piece;
        }

        public int Size
        {
            get { return 16 * 1024; }
        }

        public int Count
        {
            get { return (piece.Size - 1) / Size + 1; }
        }

        public IEnumerator<TorrentBlock> GetEnumerator()
        {
            int last = Count - 1;
            long offset = piece.Offset;

            for (int i = 0; i < Count; i++)
            {
                yield return new TorrentBlock(offset, GetBlockSize(last, i));
                offset += GetBlockSize(last, i);
            }
        }

        private int GetBlockSize(int last, int index)
        {
            if (index < last)
            {
                return Size;
            }

            if (piece.Size % Size == 0)
            {
                return Size;
            }

            return piece.Size % Size;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}