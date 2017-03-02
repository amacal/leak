using Leak.Tracker.Get;

namespace Leak.Client.Tracker
{
    public interface TrackerRuntime
    {
        TrackerGetService Service { get; }

        void Start(TrackerGetHooks hooks);

        void Stop();
    }
}