using System;

namespace Leak.Omnibus
{
    public class OmnibusConfiguration
    {
        public OmnibusConfiguration()
        {
            LeaseDuration = TimeSpan.FromSeconds(15);
            SchedulerThreshold = 20;
        }

        public TimeSpan LeaseDuration { get; set; }

        public int SchedulerThreshold { get; set; }
    }
}