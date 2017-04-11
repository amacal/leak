﻿using System;
using Leak.Networking.Core;

namespace Leak.Peer.Communicator.Messages
{
    public class RequestOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly Request request;

        public RequestOutgoingMessage(Request request)
        {
            this.request = request;
        }

        public int Length
        {
            get { return 17; }
        }

        public void ToBytes(DataBlock block)
        {
            byte[] data = new byte[17];

            data[3] = 13;
            data[4] = 6;
            data[5] = (byte)((request.Block.Piece.Index >> 24) & 255);
            data[6] = (byte)((request.Block.Piece.Index >> 16) & 255);
            data[7] = (byte)((request.Block.Piece.Index >> 8) & 255);
            data[8] = (byte)(request.Block.Piece.Index & 255);
            data[9] = (byte)((request.Block.Offset >> 24) & 255);
            data[10] = (byte)((request.Block.Offset >> 16) & 255);
            data[11] = (byte)((request.Block.Offset >> 8) & 255);
            data[12] = (byte)(request.Block.Offset & 255);
            data[13] = (byte)((request.Block.Size >> 24) & 255);
            data[14] = (byte)((request.Block.Size >> 16) & 255);
            data[15] = (byte)((request.Block.Size >> 8) & 255);
            data[16] = (byte)(request.Block.Size & 255);

            block.With((buffer, offset, count) =>
            {
                Array.Copy(data, 0, buffer, offset, Length);
            });
        }

        public void Release()
        {
        }
    }
}