using System.Collections.Generic;

namespace Leak.Core.Extensions
{
    public class ExtenderConfiguration
    {
        public ExtenderCallback Callback { get; set; }

        public ExtenderExtensionCollection Extensions { get; set; }

        public List<ExtenderHandler> Handlers { get; set; }
    }
}