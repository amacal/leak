using Leak.Extensions;
using System.Collections.Generic;

namespace Leak.Glue
{
    public class GlueConfiguration
    {
        public GlueConfiguration()
        {
            Plugins = new List<MorePlugin>();
        }

        public List<MorePlugin> Plugins;

        public int? Pieces;
    }
}