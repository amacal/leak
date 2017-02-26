using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetUdpTask : LeakTask<TrackerGetContext>
    {
        public void Execute(TrackerGetContext context)
        {
            context.UdpService.Schedule();
        }
    }
}