using System;
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
            int size = piece.Index.Size;
            byte[] result = new byte[13 + size];

            result[4] = 0x07;

            Bytes.Write(piece.Index.Size + 9, result, 0);
            Bytes.Write(piece.Index.Piece.Index, result, 5);
            Bytes.Write(piece.Index.Offset, result, 9);

            piece.Data.Write((buffer, offset, count) =>
            {
                Array.Copy(buffer, offset, result, 13, size);
            });

            return result;
        }
    }
}