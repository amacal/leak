using Leak.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Leak.Testing
{
    public class PipelineSimulator : PipelineService
    {
        private readonly List<Action> ticks;
        private readonly List<LeakPipelineTrigger> triggers;
        private readonly ManualResetEvent synchronizer;

        public PipelineSimulator()
        {
            ticks = new List<Action>();
            triggers = new List<LeakPipelineTrigger>();
            synchronizer = new ManualResetEvent(false);
        }

        void PipelineService.Register(LeakPipelineTrigger trigger)
        {
            trigger.Register(synchronizer);
            triggers.Add(trigger);
        }

        void PipelineService.Register(TimeSpan period, Action callback)
        {
            ticks.Add(callback);
        }

        void PipelineService.Remove(Action callback)
        {
            ticks.Remove(callback);
        }

        public void Process()
        {
            foreach (LeakPipelineTrigger trigger in triggers)
            {
                trigger.Execute();
            }
        }

        public void Tick()
        {
            foreach (Action tick in ticks)
            {
                tick.Invoke();
            }
        }
    }
}