using System;

namespace Leak.Core.IO
{
    public class MetainfoPiece
    {
        private readonly byte[] data;
        private readonly int index;

        public MetainfoPiece(byte[] data, int index)
        {
            this.data = data;
            this.index = index;
        }

        public int Index
        {
            get { return index; }
        }

        public byte[] Hash
        {
            get
            {
                byte[] result = new byte[20];
                Array.Copy(data, index * 20, result, 0, 20);
                return result;
            }
        }
    }
}