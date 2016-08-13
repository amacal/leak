namespace Leak.Core.Cando.Metadata
{
    public class MetadataData
    {
        private readonly int piece;
        private readonly int size;
        private readonly byte[] payload;

        public MetadataData(int piece, int size, byte[] payload)
        {
            this.piece = piece;
            this.size = size;
            this.payload = payload;
        }

        public int Piece
        {
            get { return piece; }
        }

        public int Size
        {
            get { return size; }
        }

        public byte[] Payload
        {
            get { return payload; }
        }
    }
}