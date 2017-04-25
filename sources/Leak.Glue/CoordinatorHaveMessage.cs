using System;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorHaveMessage : SenderMessage
    {
        private readonly PieceInfo piece;

        public CoordinatorHaveMessage(PieceInfo piece)
        {
            this.piece = piece;
        }

        public string Type
        {
            get { return "have"; }
        }

        public NetworkOutgoingMessage Apply(byte id)
        {
            return new Instance(id, piece);
        }

        public void Release()
        {
        }

        private class Instance : NetworkOutgoingMessage
        {
            private readonly byte id;
            private readonly PieceInfo piece;

            public Instance(byte id, PieceInfo piece)
            {
                this.id = id;
                this.piece = piece;
            }

            public int Length
            {
                get { return 9; }
            }

            public void ToBytes(DataBlock block)
            {
                byte lowest = (byte)(piece.Index % 256);
                byte highest = (byte)(piece.Index / 256);

                block.With((buffer, offset, count) =>
                {
                    Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x05, id, 0x00, 0x00, highest, lowest }, 0, buffer, offset, Length);
                });
            }

            public void Release()
            {
            }
        }
    }
}