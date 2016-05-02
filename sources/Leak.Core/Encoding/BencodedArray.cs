using System.Collections.Generic;

namespace Leak.Core.Encoding
{
    public class BencodedArray : BencodedValue
    {
        private readonly List<BencodedValue> items;

        public BencodedArray()
        {
            this.items = new List<BencodedValue>();
        }

        public void Add(BencodedValue value)
        {
            items.Add(value);
        }

        public BencodedValue[] Items
        {
            get { return items.ToArray(); }
        }
    }
}