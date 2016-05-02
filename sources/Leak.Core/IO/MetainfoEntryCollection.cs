using Leak.Core.Encoding;
using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class MetainfoEntryCollection : IEnumerable<MetainfoEntry>
    {
        private readonly BencodedValue data;

        public MetainfoEntryCollection(BencodedValue data)
        {
            this.data = data;
        }

        public IEnumerator<MetainfoEntry> GetEnumerator()
        {
            return data.Find("files", GetEnumerator);
        }

        private IEnumerator<MetainfoEntry> GetEnumerator(BencodedValue files)
        {
            if (files == null)
            {
                yield return new MetainfoEntry(data);
                yield break;
            }

            foreach (BencodedValue value in files.AllItems())
            {
                yield return new MetainfoEntry(value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}