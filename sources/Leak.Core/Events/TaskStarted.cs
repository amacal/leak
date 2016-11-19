using Leak.Core.Common;
using Leak.Core.Spartan;

namespace Leak.Core.Events
{
    public class TaskStarted
    {
        public FileHash Hash;

        public SpartanTasks Task;
    }
}