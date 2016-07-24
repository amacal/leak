using Leak.Core.Extensions.Metadata;
using System.Collections.Generic;

namespace Leak.Core.Extensions
{
    public class ExtenderConfiguration
    {
        public ExtenderCallback Callback { get; set; }

        public ExtenderExtensionCollection Extensions { get; set; }

        public List<MetadataHandler> Handlers { get; set; }
    }
}