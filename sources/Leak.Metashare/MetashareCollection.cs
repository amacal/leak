using System.Collections.Generic;
using Leak.Common;

namespace Leak.Meta.Share
{
    public class MetashareCollection
    {
        private readonly Dictionary<int, List<MetashareEntry>> byPiece;

        public MetashareCollection()
        {
            byPiece = new Dictionary<int, List<MetashareEntry>>();
        }

        public void Register(PeerHash peer, int piece)
        {
            List<MetashareEntry> entries;

            if (byPiece.TryGetValue(piece, out entries) == false)
            {
                entries = new List<MetashareEntry>();
                byPiece.Add(piece, entries);
            }

            entries.Add(new MetashareEntry
            {
                Piece = piece,
                Peer = peer
            });
        }

        public MetashareEntry[] Remove(int piece)
        {
            List<MetashareEntry> entries;

            if (byPiece.TryGetValue(piece, out entries) == false)
            {
                entries = new List<MetashareEntry>();
            }
            else
            {
                byPiece.Remove(piece);
            }

            return entries.ToArray();
        }
    }
}