using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class MetainfoFileConfiguration
    {
        public MetainfoFileConfiguration()
        {
            Trackers = new List<string>();
            Includes = new List<string>();
        }

        public string Path { get; set; }

        public int PieceLength { get; set; }

        public List<string> Trackers { get; set; }

        public List<string> Includes { get; set; }
    }
}