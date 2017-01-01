using Leak.Common;
using Leak.Events;

namespace Leak.Spartan
{
    public static class SpartanExtensions
    {
        public static void CallTaskStarted(this SpartanHooks hooks, FileHash hash, Goal task)
        {
            hooks.OnTaskStarted?.Invoke(new TaskStarted
            {
                Hash = hash,
                Task = task
            });
        }

        public static void CallTaskCompleted(this SpartanHooks hooks, FileHash hash, Goal task)
        {
            hooks.OnTaskCompleted?.Invoke(new TaskCompleted
            {
                Hash = hash,
                Task = task
            });
        }
    }
}