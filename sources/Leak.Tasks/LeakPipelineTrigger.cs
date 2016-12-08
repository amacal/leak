using System.Threading;

namespace Leak.Tasks
{
    public interface LeakPipelineTrigger
    {
        void Register(ManualResetEvent watch);

        void Execute();
    }
}