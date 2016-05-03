namespace Leak.Core.Net
{
    public abstract class TrackerClient
    {
        public abstract TrackerResonse Announce(PeerHandshake handshake);
    }
}