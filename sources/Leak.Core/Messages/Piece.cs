namespace Leak.Core.Messages
{
    public class Piece
    {
        private readonly int index;
        private readonly int offset;
        private readonly DataBlock data;

        public Piece(int index, int offset, DataBlock data)
        {
            this.index = index;
            this.offset = offset;
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
            get { return data.Size; }
        }

        public DataBlock Data
        {
            get { return data; }
        }
    }
}