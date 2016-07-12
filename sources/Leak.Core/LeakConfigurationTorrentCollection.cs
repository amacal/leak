using System.Collections.Generic;

namespace Leak.Core
{
    public class LeakConfigurationTorrentCollection
    {
        public LeakConfigurationTorrentCollection()
        {
            Schedules = new LeakConfigurationScheduleCollection();
            Peers = new List<LeakConfigurationPeer>();
        }

        public LeakConfigurationScheduleCollection Schedules { get; set; }

        public List<LeakConfigurationPeer> Peers { get; set; }
    }
}