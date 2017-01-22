using System;

namespace Leak.Tasks
{
    public interface PipelineService
    {
        void Register(LeakPipelineTrigger trigger);

        void Register(TimeSpan period, Action callback);

        void Remove(Action callback);
    }
}