using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class PieceIncomingMessage
    {
        private readonly DataBlock data;

        private readonly int piece;
        private readonly int offset;

        public PieceIncomingMessage(DataBlock block)
        {
            data = block.Scope(8);
            piece = block[3] + block[2] * 256 + block[1] * 256 * 256;
            offset = block[7] + block[6] * 256 + block[5] * 256 * 256;
        }

        public int Piece
        {
            get { return piece; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Size
        {
            get { return data.Size; }
        }

        public Piece ToPiece()
        {
            return new Piece(new BlockIndex(piece, offset, data.Size), data);
        }
    }
}