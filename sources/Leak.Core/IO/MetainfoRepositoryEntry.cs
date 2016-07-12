using System;
using System.Security.Cryptography;

namespace Leak.Core.IO
{
    public class MetainfoRepositoryEntry
    {
        private readonly byte[] hash;
        private byte[] pieces;
        private int? size;

        public MetainfoRepositoryEntry(byte[] hash)
        {
            this.hash = hash;
            this.pieces = new byte[0];
        }

        public MetainfoRepositoryEntry(byte[] hash, byte[] pieces)
        {
            this.hash = hash;
            this.pieces = pieces ?? new byte[0];
        }

        public byte[] Hash
        {
            get { return hash; }
        }

        public int GetTotalSize()
        {
            return size ?? pieces.Length;
        }

        public void SetTotalSize(int size)
        {
            this.size = size;
        }

        public byte[] GetData(int piece)
        {
            int offset = piece * 16384;
            int length = Math.Min(16384, pieces.Length - offset);

            if (pieces.Length <= offset)
                return null;

            return pieces.ToBytes(offset, length);
        }

        public void SetData(int piece, byte[] data)
        {
            int offset = piece * 16384;
            int length = data.Length;

            Bytes.Extend(ref pieces, offset + length);
            Array.Copy(data, 0, pieces, pieces.Length - length, length);
        }

        public bool IsCompleted()
        {
            using (SHA1 algorithm = SHA1.Create())
            {
                return Bytes.Equals(hash, algorithm.ComputeHash(pieces));
            }
        }

        public byte[] ToArray()
        {
            return pieces;
        }
    }
}