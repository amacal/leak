using Leak.Core.Encoding;
using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class MetainfoPieceCollection : IEnumerable<MetainfoPiece>
    {
        private readonly BencodedValue data;

        public MetainfoPieceCollection(BencodedValue data)
        {
            this.data = data;
        }

        public int Size
        {
            get
            {
                return (int)data.Find("piece length", x => x.ToNumber());
            }
        }

        public int Count
        {
            get
            {
                return data.Find("pieces", GetCount);
            }
        }

        private int GetCount(BencodedValue pieces)
        {
            return pieces.ToHex().Length / 20;
        }

        public IEnumerator<MetainfoPiece> GetEnumerator()
        {
            return data.Find("pieces", GetEnumerator);
        }

        private IEnumerator<MetainfoPiece> GetEnumerator(BencodedValue pieces)
        {
            byte[] bytes = pieces.ToHex();

            for (int i = 0; i < bytes.Length; i += 20)
            {
                yield return new MetainfoPiece(bytes, i / 20);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}