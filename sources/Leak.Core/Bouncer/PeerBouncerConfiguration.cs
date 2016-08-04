namespace Leak.Core.Bouncer
{
    public class PeerBouncerConfiguration
    {
        public int MaximumNumberOfConnections { get; set; }

        public PeerBouncerCallback Callback { get; set; }
    }
}