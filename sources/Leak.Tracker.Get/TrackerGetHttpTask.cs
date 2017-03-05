using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetHttpTask : LeakTask<TrackerGetContext>
    {
        public void Execute(TrackerGetContext context)
        {
            context.Http.Schedule();
        }
    }
}