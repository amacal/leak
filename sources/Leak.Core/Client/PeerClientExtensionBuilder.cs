using Leak.Core.Extensions;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientExtensionBuilder
    {
        private readonly List<PeerClientExtension> extensions;

        public PeerClientExtensionBuilder()
        {
            this.extensions = new List<PeerClientExtension>();
        }

        public void Metadata()
        {
            extensions.Add(new PeerClientExtensionMetadata());
        }

        public void Register(ICollection<ExtenderHandler> handlers, PeerClientExtensionContext context)
        {
            foreach (PeerClientExtension extension in extensions)
            {
                extension.Register(handlers, context);
            }
        }

        public ExtenderExtensionCollection Build()
        {
            List<string> keys = new List<string>();
            List<ExtenderExtension> mapping = new List<ExtenderExtension>();

            foreach (PeerClientExtension extension in extensions)
            {
                extension.Register(keys);
            }

            for (byte i = 1; i <= keys.Count; i++)
            {
                mapping.Add(new ExtenderExtension(i, keys[i - 1]));
            }

            return new ExtenderExtensionCollection(mapping.ToArray());
        }
    }
}