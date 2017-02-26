using Leak.Common;

namespace Leak.Tracker.Get
{
    public interface TrackerGetProxy
    {
        void Announce(FileHash hash);
    }
}