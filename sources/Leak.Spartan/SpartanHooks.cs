using Leak.Events;
using System;

namespace Leak.Spartan
{
    public class SpartanHooks
    {
        /// <summary>
        /// Called when the task was started and no other task is ongoing.
        /// </summary>
        public Action<TaskStarted> OnTaskStarted;

        /// <summary>
        /// Called when the task was completed and another pending task
        /// can be probably started.
        /// </summary>
        public Action<TaskCompleted> OnTaskCompleted;
    }
}