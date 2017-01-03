using Leak.Common;

namespace Leak.Communicator.Messages
{
    public class PieceOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly Piece piece;

        public PieceOutgoingMessage(Piece piece)
        {
            this.piece = piece;
        }

        public int Length
        {
            get { return 13 + piece.Index.Size; }
        }

        public byte[] ToBytes()
        {
            byte[] result =
            {
                0x00, 0x00, 0x00, 0x00, 0x07,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            Bytes.Write(piece.Index.Size + 9, result, 0);
            Bytes.Write(piece.Index.Piece, result, 5);
            Bytes.Write(piece.Index.Offset, result, 9);
            Bytes.Append(ref result, result);

            return result;
        }
    }
}