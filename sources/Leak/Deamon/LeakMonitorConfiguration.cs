namespace Leak.Deamon
{
    public class LeakMonitorConfiguration
    {
        public string Directory { get; set; }

        public LeakMonitorCallback Callback { get; set; }
    }
}