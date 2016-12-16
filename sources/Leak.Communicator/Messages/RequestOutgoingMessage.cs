using Leak.Common;

namespace Leak.Communicator.Messages
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

        public byte[] ToBytes()
        {
            byte[] data = new byte[17];

            data[3] = 13;
            data[4] = 6;
            data[5] = (byte)((request.Piece >> 24) & 255);
            data[6] = (byte)((request.Piece >> 16) & 255);
            data[7] = (byte)((request.Piece >> 8) & 255);
            data[8] = (byte)(request.Piece & 255);
            data[9] = (byte)((request.Offset >> 24) & 255);
            data[10] = (byte)((request.Offset >> 16) & 255);
            data[11] = (byte)((request.Offset >> 8) & 255);
            data[12] = (byte)(request.Offset & 255);
            data[13] = (byte)((request.Size >> 24) & 255);
            data[14] = (byte)((request.Size >> 16) & 255);
            data[15] = (byte)((request.Size >> 8) & 255);
            data[16] = (byte)(request.Size & 255);

            return data;
        }
    }
}