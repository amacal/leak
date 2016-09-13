namespace Leak.Core.Messages
{
    public class HaveMessage
    {
        private readonly byte[] data;

        public HaveMessage(byte[] data)
        {
            this.data = data;
        }

        public int Piece
        {
            get { return Bytes.ReadInt32(data, 0); }
        }
    }
}