using System;
using System.Collections.Generic;
using System.Threading;

namespace Leak.Core.Core
{
    public class LeakPipeline
    {
        private readonly Thread worker;
        private readonly List<LeakPipelineTimer> ticks;

        private WaitHandle[] handles;
        private LeakPipelineTrigger[] triggers;

        public LeakPipeline()
        {
            worker = new Thread(Execute);
            ticks = new List<LeakPipelineTimer>();

            handles = new WaitHandle[0];
            triggers = new LeakPipelineTrigger[0];
        }

        public void Start()
        {
            worker.Start();
        }

        public void Register(TimeSpan period, Action callback)
        {
            ticks.Add(new LeakPipelineTimer(period, callback));
        }

        public void Remove(Action callback)
        {
        }

        public void Register(LeakPipelineTrigger trigger)
        {
            ManualResetEvent handle = new ManualResetEvent(false);
            WaitHandle[] newHandles = new WaitHandle[handles.Length + 1];
            LeakPipelineTrigger[] newTriggers = new LeakPipelineTrigger[triggers.Length + 1];

            Array.Copy(handles, newHandles, handles.Length);
            Array.Copy(triggers, newTriggers, triggers.Length);

            newHandles[handles.Length] = handle;
            newTriggers[triggers.Length] = trigger;
            newTriggers[triggers.Length].Register(handle);

            triggers = newTriggers;
            handles = newHandles;
        }

        private void Execute()
        {
            while (true)
            {
                int found = WaitHandle.WaitAny(handles, 50);
                DateTime now = DateTime.Now;

                foreach (LeakPipelineTimer tick in ticks)
                {
                    tick.Execute(now);
                }

                if (WaitHandle.WaitTimeout != found)
                {
                    foreach (LeakPipelineTrigger trigger in triggers)
                    {
                        trigger.Execute();
                    }
                }
            }
        }
    }
}