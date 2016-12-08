using Leak.Common;

namespace Leak.Core.Messages
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
            get { return 13 + piece.Size; }
        }

        public byte[] ToBytes()
        {
            byte[] result =
            {
                0x00, 0x00, 0x00, 0x00, 0x07,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            Bytes.Write(piece.Size + 9, result, 0);
            Bytes.Write(piece.Index, result, 5);
            Bytes.Write(piece.Offset, result, 9);
            Bytes.Append(ref result, result);

            return result;
        }
    }
}