using Leak.Core.IO;

namespace Leak.Core
{
    public class LeakConfigurationSchedule
    {
        internal byte[] Hash { get; set; }

        internal MetainfoFile Metainfo { get; set; }

        internal LeakConfigurationScheduleOperation Operation { get; set; }
    }
}