using Leak.Core.Encoding;
using System;
using System.IO;

namespace Leak.Core.IO
{
    public class MetainfoFileEntry
    {
        private readonly BencodedValue data;

        public MetainfoFileEntry(BencodedValue data)
        {
            this.data = data;
        }

        public string Name
        {
            get { return GetInSingleFileMode() ?? GetInMultipleFileMode(); }
        }

        private string GetInSingleFileMode()
        {
            return data.Find("name", x => x.ToText());
        }

        private string GetInMultipleFileMode()
        {
            return data.Find("path", path =>
            {
                string[] items = data.AllTexts();
                string separator = Path.DirectorySeparatorChar.ToString();

                return String.Join(separator, items);
            });
        }
    }
}