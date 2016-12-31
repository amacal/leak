using Leak.Common;
using Leak.Metaget;
using Leak.Repository;
using Leak.Retriever;

namespace Leak.Spartan
{
    public class SpartanFacts
    {
        private Goal completed;
        private Goal ongoing;
        private Goal pending;

        public SpartanFacts(SpartanConfiguration configuration)
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

        public Metainfo Metainfo;

        public Bitfield Bitfield;

        public RepositoryService Repository;

        public RetrieverService Retriever;
    }
}