using System;
using Leak.Common;
using Leak.Networking.Core;

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

        public void ToBytes(DataBlock block)
        {
            block.With((buffer, offset, count) =>
            {
                buffer[4 + offset] = 0x07;

                Bytes.Write(piece.Index.Size + 9, buffer, offset + 0);
                Bytes.Write(piece.Index.Piece.Index, buffer, offset + 5);
                Bytes.Write(piece.Index.Offset, buffer, offset + 9);

                piece.Data.With((buffer2, offset2, count2) =>
                {
                    Array.Copy(buffer2, offset2, buffer, offset + 13, piece.Index.Size);
                });
            });
        }

        public void Release()
        {
            piece.Data.Release();
        }
    }
}