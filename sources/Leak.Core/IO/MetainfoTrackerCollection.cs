using System;
using System.Collections;
using System.Collections.Generic;
using Leak.Core.Bencoding;

namespace Leak.Core.IO
{
    public class MetainfoTrackerCollection : IEnumerable<MetainfoTracker>
    {
        private readonly BencodedValue data;

        public MetainfoTrackerCollection(BencodedValue data)
        {
            this.data = data;
        }

        public IEnumerator<MetainfoTracker> GetEnumerator()
        {
            ICollection<string> result = new HashSet<string>();

            data.Find("announce", node =>
            {
                if (node != null)
                {
                    result.Add(node.ToText());
                }

                return node;
            });

            data.Find("announce-list", node =>
            {
                if (node != null)
                {
                    foreach (string text in node.AllTexts())
                    {
                        if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
                        {
                            result.Add(text);
                        }
                    }
                }

                return node;
            });

            foreach (string uri in result)
            {
                yield return new MetainfoTracker(new Uri(uri, UriKind.Absolute));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}