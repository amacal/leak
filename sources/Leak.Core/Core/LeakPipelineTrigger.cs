using System.Threading;

namespace Leak.Core.Core
{
    public interface LeakPipelineTrigger
    {
        void Register(ManualResetEvent watch);

        void Execute();
    }
}