using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskQueue
    {
        private readonly PeerClientTaskContext context;

        private readonly Dictionary<FileHash, List<PeerClientTask>> items;
        private readonly Dictionary<PeerClientTask, PeerClientTaskCallback> running;

        public PeerClientTaskQueue(PeerClientTaskContext context)
        {
            this.context = context;

            items = new Dictionary<FileHash, List<PeerClientTask>>();
            running = new Dictionary<PeerClientTask, PeerClientTaskCallback>();
        }

        public void Register(PeerClientTask task)
        {
            FileHash hash = task.Hash;
            List<PeerClientTask> tasks;

            if (items.TryGetValue(hash, out tasks) == false)
            {
                tasks = new List<PeerClientTask>();
                items.Add(hash, tasks);
            }

            tasks.Add(task);
            task = tasks[0];

            if (running.ContainsKey(task) == false)
            {
                running.Add(task, task.Start(context));
            }
        }

        public void Complete(PeerClientTask task)
        {
            FileHash hash = task.Hash;
            List<PeerClientTask> tasks;

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

        public void Notify(Action<PeerClientTaskCallback> callback)
        {
            foreach (PeerClientTaskCallback target in running.Values)
            {
                callback.Invoke(target);
            }
        }
    }
}