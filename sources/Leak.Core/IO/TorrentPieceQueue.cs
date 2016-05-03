using System;
using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class TorrentPieceQueue : IEnumerable<TorrentPiece>
    {
        private readonly Dictionary<TorrentPiece, DateTime> pieces;

        public TorrentPieceQueue()
        {
            this.pieces = new Dictionary<TorrentPiece, DateTime>();
        }

        public void Add(TorrentPiece piece)
        {
            pieces[piece] = DateTime.Now;
        }

        public void Remove(TorrentPiece piece)
        {
            pieces.Remove(piece);
        }

        public IEnumerator<TorrentPiece> GetEnumerator()
        {
            foreach (var entry in pieces)
            {
                if (entry.Value.AddMinutes(5) > DateTime.Now)
                {
                    yield return entry.Key;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}