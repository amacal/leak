using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Encoding
{
    public class BencodedDictionary : BencodedValue
    {
        private readonly List<Tuple<BencodedText, BencodedValue>> items;

        public BencodedDictionary()
        {
            this.items = new List<Tuple<BencodedText, BencodedValue>>();
        }

        public void Add(BencodedText key, BencodedValue value)
        {
            items.Add(Tuple.Create(key, value));
        }

        public BencodedValue Find(string name)
        {
            foreach (Tuple<BencodedText, BencodedValue> entry in items)
            {
                if (entry.Item1.Value == name)
                {
                    return entry.Item2;
                }
            }

            return null;
        }

        public BencodedText[] Keys
        {
            get { return items.Select(x => x.Item1).ToArray(); }
        }

        public BencodedValue[] Values
        {
            get { return items.Select(x => x.Item2).ToArray(); }
        }
    }
}