using Leak.Core.Encoding;
using System.Collections;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class MetainfoFileEntryCollection : IEnumerable<MetainfoFileEntry>
    {
        private readonly BencodedValue data;

        public MetainfoFileEntryCollection(BencodedValue data)
        {
            this.data = data;
        }

        public IEnumerator<MetainfoFileEntry> GetEnumerator()
        {
            return data.Find("files", files => GetEnumerator(files));
        }

        private IEnumerator<MetainfoFileEntry> GetEnumerator(BencodedValue files)
        {
            if (files == null)
            {
                yield return new MetainfoFileEntry(data);
                yield break;
            }

            foreach (BencodedValue value in files.AllItems())
            {
                yield return new MetainfoFileEntry(value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}