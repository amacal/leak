using System;

namespace Leak.Tasks
{
    public static class PipelineExtensions
    {
        public static void Register(this PipelineService pipeline, int milliseconds, Action callback)
        {
            pipeline.Register(TimeSpan.FromMilliseconds(milliseconds), callback);
        }
    }
}