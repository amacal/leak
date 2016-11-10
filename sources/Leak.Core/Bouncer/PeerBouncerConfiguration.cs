namespace Leak.Core.Bouncer
{
    public class PeerBouncerConfiguration
    {
        public int Connections { get; set; }

        public string[] Countries { get; set; }

        public PeerBouncerCallback Callback { get; set; }
    }
}