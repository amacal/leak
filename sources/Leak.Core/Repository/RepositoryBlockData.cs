using Leak.Core.Messages;
using System;

namespace Leak.Core.Repository
{
    public class RepositoryBlockData
    {
        private readonly int piece;
        private readonly int offset;
        private readonly DataBlock data;

        public RepositoryBlockData(int piece, int offset, DataBlock data)
        {
            this.piece = piece;
            this.offset = offset;
            this.data = data;
        }

        public int Piece
        {
            get { return piece; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Length
        {
            get { return data.Size; }
        }

        public void Write(Action<byte[], int, int> stream)
        {
            data.Write(stream);
        }
    }
}