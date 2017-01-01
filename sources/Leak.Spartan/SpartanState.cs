using Leak.Common;

namespace Leak.Spartan
{
    public class SpartanState
    {
        private Goal completed;
        private Goal ongoing;
        private Goal pending;

        public SpartanState(SpartanConfiguration configuration)
        {
            completed = Goal.None;
            ongoing = Goal.None;
            pending = configuration.Goal;
        }

        public bool CanStart(Goal tasks)
        {
            return pending.HasFlag(tasks) && ongoing == Goal.None;
        }

        public bool IsOngoing(Goal tasks)
        {
            return ongoing.HasFlag(tasks);
        }

        public void Start(Goal tasks)
        {
            pending &= ~tasks;
            ongoing |= tasks;
        }

        public void Complete(Goal tasks)
        {
            ongoing &= ~tasks;
            completed |= tasks;
        }
    }
}