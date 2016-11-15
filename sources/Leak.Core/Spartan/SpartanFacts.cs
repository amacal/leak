using Leak.Core.Metaget;
using Leak.Core.Repository;
using Leak.Core.Retriever;

namespace Leak.Core.Spartan
{
    public class SpartanFacts
    {
        private SpartanGoal completed;
        private SpartanGoal ongoing;
        private SpartanGoal pending;

        public SpartanFacts(SpartanConfiguration configuration)
        {
            completed = SpartanGoal.None;
            ongoing = SpartanGoal.None;
            pending = configuration.Goal;
        }

        public bool CanStart(SpartanGoal goal)
        {
            return pending.HasFlag(goal) && ongoing == SpartanGoal.None;
        }

        public void Start(SpartanGoal goal)
        {
            pending &= ~goal;
            ongoing |= goal;
        }

        public void Complete(SpartanGoal goal)
        {
            ongoing &= ~goal;
            completed |= goal;
        }

        public MetagetService MetaGet;

        public RepositoryService Repository;

        public RetrieverService Retriever;
    }
}