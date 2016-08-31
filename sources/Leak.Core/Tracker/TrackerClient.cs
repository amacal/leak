namespace Leak.Core.Tracker
{
    public interface TrackerClient
    {
        TrackerAnnounce Announce(TrackerRequest request);
    }
}