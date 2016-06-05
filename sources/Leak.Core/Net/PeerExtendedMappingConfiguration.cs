using System;
using System.Collections.Generic;

namespace Leak.Core.Net
{
    public class PeerExtendedMappingConfiguration
    {
        public PeerExtendedMappingConfiguration()
        {
            Extensions = new List<Tuple<string, int>>();
            Properties = new List<Tuple<string, object>>();
        }

        public List<Tuple<string, int>> Extensions { get; set; }

        public List<Tuple<string, object>> Properties { get; set; }
    }
}