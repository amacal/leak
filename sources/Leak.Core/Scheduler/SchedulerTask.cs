using Leak.Core.Common;

namespace Leak.Core.Scheduler
{
    public interface SchedulerTask
    {
        FileHash Hash { get; }

        SchedulerTaskCallback Start(SchedulerContext context);
    }
}