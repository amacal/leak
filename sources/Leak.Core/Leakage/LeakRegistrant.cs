using Leak.Common;
using System.Collections.Generic;

namespace Leak.Core.Leakage
{
    public class LeakRegistrant
    {
        public LeakRegistrant()
        {
            Trackers = new List<string>();
            Peers = new List<string>();
        }

        public FileHash Hash;

        public string Destination;

        public List<string> Trackers;

        public List<string> Peers;
    }
}