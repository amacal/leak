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

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            return factory.Pooled(piece.Index.Size + 13, (buffer, offset, count) =>
            {
                buffer[4 + offset] = 0x07;

                Bytes.Write(piece.Index.Size + 9, buffer, offset);
                Bytes.Write(piece.Index.Piece.Index, buffer, offset + 5);
                Bytes.Write(piece.Index.Offset, buffer, offset + 9);

                piece.Data.With((buffer2, offset2, count2) =>
                {
                    Array.Copy(buffer2, offset2, buffer, offset + 13, piece.Index.Size);
                });
            });
        }
    }
}