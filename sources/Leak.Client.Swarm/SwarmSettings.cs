namespace Leak.Client.Swarm
{
    public class SwarmSettings
    {
        public SwarmSettings()
        {
            Listener = false;
            Connector = true;
        }

        public bool Listener { get; set; }
        public int? ListenerPort { get; set; }

        public bool Connector { get; set; }
    }
}