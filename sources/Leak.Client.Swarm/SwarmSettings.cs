namespace Leak.Client.Swarm
{
    public class SwarmSettings
    {
        public SwarmSettings()
        {
            Listener = false;
            Connector = true;
            Strategy = "rarest-first";
        }

        public bool Listener { get; set; }
        public int? ListenerPort { get; set; }

        public bool Connector { get; set; }
        public SwarmFilter Filter { get; set; }

        public string Strategy { get; set; }
    }
}