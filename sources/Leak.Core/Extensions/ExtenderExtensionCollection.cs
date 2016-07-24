using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Extensions
{
    public class ExtenderExtensionCollection : IEnumerable<ExtenderExtension>
    {
        private readonly ExtenderExtension[] items;

        public ExtenderExtensionCollection()
        {
            this.items = new ExtenderExtension[0];
        }

        public ExtenderExtensionCollection(ExtenderExtension[] items)
        {
            this.items = items;
        }

        public byte Translate(string name)
        {
            foreach (ExtenderExtension extension in items)
            {
                if (extension.Name == name)
                {
                    return extension.Id;
                }
            }

            throw new NotSupportedException();
        }

        public string Translate(byte id)
        {
            foreach (ExtenderExtension extension in items)
            {
                if (extension.Id == id)
                {
                    return extension.Name;
                }
            }

            return null;
        }

        public IEnumerator<ExtenderExtension> GetEnumerator()
        {
            return items.OfType<ExtenderExtension>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}