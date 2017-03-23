using Leak.Completion;
using Leak.Files;
using Leak.Tasks;

namespace Leak.Client
{
    public interface Runtime
    {
        PipelineService Pipeline { get; }

        FileFactory Files { get; }

        CompletionWorker Worker { get; }

        void Start();

        void Stop();
    }
}