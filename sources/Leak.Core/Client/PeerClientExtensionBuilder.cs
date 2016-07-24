using Leak.Core.Extensions;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientExtensionBuilder
    {
        private readonly List<ExtenderExtension> mapping;

        public PeerClientExtensionBuilder()
        {
            this.mapping = new List<ExtenderExtension>();
        }

        public void Register(string name, byte id)
        {
            mapping.Add(new ExtenderExtension(id, name));
        }

        public ExtenderExtensionCollection Build()
        {
            return new ExtenderExtensionCollection(mapping.ToArray());
        }
    }
}