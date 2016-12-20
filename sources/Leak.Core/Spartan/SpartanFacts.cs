using Leak.Common;
using Leak.Metaget;
using Leak.Repository;
using Leak.Retriever;

namespace Leak.Core.Spartan
{
    public class SpartanFacts
    {
        private SpartanTasks completed;
        private SpartanTasks ongoing;
        private SpartanTasks pending;

        public SpartanFacts(SpartanConfiguration configuration)
        {
            completed = SpartanTasks.None;
            ongoing = SpartanTasks.None;
            pending = configuration.Tasks;
        }

        public bool CanStart(SpartanTasks tasks)
        {
            return pending.HasFlag(tasks) && ongoing == SpartanTasks.None;
        }

        public bool IsOngoing(SpartanTasks tasks)
        {
            return ongoing.HasFlag(tasks);
        }

        public void Start(SpartanTasks tasks)
        {
            pending &= ~tasks;
            ongoing |= tasks;
        }

        public void Complete(SpartanTasks tasks)
        {
            ongoing &= ~tasks;
            completed |= tasks;
        }

        public Metainfo Metainfo;

        public Bitfield Bitfield;

        public MetagetService MetaGet;

        public RepositoryService Repository;

        public RetrieverService Retriever;
    }
}