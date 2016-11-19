using Leak.Core.Common;
using Leak.Core.Spartan;

namespace Leak.Core.Events
{
    public class TaskCompleted
    {
        public FileHash Hash;

        public SpartanTasks Task;
    }
}