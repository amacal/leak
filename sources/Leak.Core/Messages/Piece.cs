namespace Leak.Core.Messages
{
    public class Piece
    {
        private readonly int index;
        private readonly int offset;
        private readonly int size;
        private readonly byte[] data;

        public Piece(int index, int offset, int size, byte[] data)
        {
            this.index = index;
            this.offset = offset;
            this.size = size;
            this.data = data;
        }

        public int Index
        {
            get { return index; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Size
        {
            get { return size; }
        }

        public byte[] Data
        {
            get { return data; }
        }
    }
}