using System;
using System.Collections.Generic;

namespace Leak.Core.Repository
{
    public class RepositoryViewCache : IDisposable
    {
        private readonly int pieceSize;
        private readonly int blockSize;
        private readonly int pieces;

        private readonly RepositoryViewEntry[] entries;
        private readonly RepositoryViewEntry[][] data;

        public RepositoryViewCache(RepositoryViewEntry[] entries, int pieceSize, int blockSize)
        {
            this.pieceSize = pieceSize;
            this.blockSize = blockSize;
            this.entries = entries;

            if (entries.Length > 0)
            {
                pieces = (int)((entries[entries.Length - 1].End - 1) / pieceSize + 1);
            }

            this.data = new RepositoryViewEntry[pieces][];

            for (int i = 0; i < pieces; i++)
            {
                long offset = i * (long)pieceSize;
                List<RepositoryViewEntry> items = new List<RepositoryViewEntry>();

                foreach (RepositoryViewEntry entry in entries)
                {
                    if (entry.End > offset && entry.Start < offset + pieceSize)
                    {
                        items.Add(entry);
                    }
                }

                data[i] = items.ToArray();
            }
        }

        public int Pieces
        {
            get { return pieces; }
        }

        public int PieceSize
        {
            get { return pieceSize; }
        }

        public int BlockSize
        {
            get { return blockSize; }
        }

        public RepositoryViewEntry[] Find(int piece)
        {
            return data[piece];
        }

        public RepositoryViewEntry[] Find(int piece, int block)
        {
            RepositoryViewEntry[] found = data[piece];

            if (found.Length == 1)
                return found;

            long offset = piece * (long)pieceSize + block * 16384;
            List<RepositoryViewEntry> constrainted = new List<RepositoryViewEntry>(1);

            foreach (RepositoryViewEntry entry in found)
            {
                if (entry.End > offset && entry.Start < offset + blockSize)
                {
                    constrainted.Add(entry);
                }
            }

            return constrainted.ToArray();
        }

        public void Flush()
        {
            foreach (RepositoryViewEntry entry in entries)
            {
                entry.File.Flush();
            }
        }

        public void Dispose()
        {
            foreach (RepositoryViewEntry entry in entries)
            {
                entry.File.Dispose();
            }
        }
    }
}