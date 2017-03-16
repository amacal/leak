using System;

namespace Leak.Data.Map
{
    public class OmnibusConfiguration
    {
        public OmnibusConfiguration()
        {
            LeaseDuration = TimeSpan.FromSeconds(15);
            SchedulerThreshold = 20;
            PoolSize = 1024;
        }

        public TimeSpan LeaseDuration { get; set; }

        public int SchedulerThreshold { get; set; }

        public int PoolSize { get; set; }
    }
}