using System;
using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class TorrentPieceView : IEnumerable<TorrentPiece>
    {
        private readonly TorrentPieceCollection collection;
        private readonly Func<TorrentPiece, bool> predicate;

        public TorrentPieceView(TorrentPieceCollection collection, Func<TorrentPiece, bool> predicate)
        {
            this.collection = collection;
            this.predicate = predicate;
        }

        public IEnumerator<TorrentPiece> GetEnumerator()
        {
            foreach (TorrentPiece piece in collection)
            {
                if (predicate.Invoke(piece))
                {
                    yield return piece;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}