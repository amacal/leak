using System;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorPieceMessage : SenderMessage
    {
        private readonly Piece piece;

        public CoordinatorPieceMessage(Piece piece)
        {
            this.piece = piece;
        }

        public string Type
        {
            get { return "piece"; }
        }

        public NetworkOutgoingMessage Apply(byte id)
        {
            return new Instance(id, piece);
        }

        public void Release()
        {
            piece.Data.Release();
        }

        private class Instance : NetworkOutgoingMessage
        {
            private readonly byte id;
            private readonly Piece piece;

            public Instance(byte id, Piece piece)
            {
                this.id = id;
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
                    buffer[4 + offset] = id;

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
}