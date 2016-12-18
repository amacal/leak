﻿using System.Collections.Generic;

namespace Leak.Glue
{
    public class GlueConfiguration
    {
        public GlueConfiguration()
        {
            Plugins = new List<GluePlugin>();
        }

        public List<GluePlugin> Plugins;

        public int? Pieces;
    }
}