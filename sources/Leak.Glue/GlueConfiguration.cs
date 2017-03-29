using Leak.Extensions;
using System.Collections.Generic;

namespace Leak.Glue
{
    public class GlueConfiguration
    {
        public GlueConfiguration()
        {
            Plugins = new List<MorePlugin>();
            AnnounceBitfield = false;
        }

        public List<MorePlugin> Plugins;
        public bool AnnounceBitfield;
    }
}