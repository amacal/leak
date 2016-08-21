using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Scheduler
{
    public class SchedulerQueue
    {
        private readonly SchedulerContext context;

        private readonly Dictionary<FileHash, List<SchedulerTask>> items;
        private readonly Dictionary<SchedulerTask, SchedulerTaskCallback> running;

        public SchedulerQueue(SchedulerContext context)
        {
            this.context = context;

            items = new Dictionary<FileHash, List<SchedulerTask>>();
            running = new Dictionary<SchedulerTask, SchedulerTaskCallback>();
        }

        public void Register(SchedulerTask task)
        {
            FileHash hash = task.Hash;
            List<SchedulerTask> tasks;

            if (items.TryGetValue(hash, out tasks) == false)
            {
                tasks = new List<SchedulerTask>();
                items.Add(hash, tasks);
            }

            tasks.Add(task);
            task = tasks[0];

            if (running.ContainsKey(task) == false)
            {
                running.Add(task, task.Start(context));
            }
        }

        public void Complete(SchedulerTask task)
        {
            FileHash hash = task.Hash;
            List<SchedulerTask> tasks;

            if (running.Remove(task))
            {
                if (items.TryGetValue(hash, out tasks))
                {
                    tasks.Remove(task);

                    if (tasks.Count > 0)
                    {
                        task = tasks[0];

                        running.Add(task, task.Start(context));
                    }
                }
            }
        }

        public void Notify(Action<SchedulerTaskCallback> callback)
        {
            foreach (SchedulerTaskCallback target in running.Values)
            {
                callback.Invoke(target);
            }
        }
    }
}