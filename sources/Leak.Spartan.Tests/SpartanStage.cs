using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Spartan.Tests
{
    public class SpartanStage
    {
        public Task Discovering;
        public Task Discovered;

        public Task Downloading;
        public Task Downloaded;

        public SpartanStage(SpartanHooks hooks)
        {
            TaskCompletionSource<bool> onDiscovering = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onDiscovered = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onDownlading = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onDownloaded = new TaskCompletionSource<bool>();

            hooks.OnTaskStarted += data =>
            {
                switch (data.Task)
                {
                    case Goal.Discover:
                        onDiscovering.SetResult(true);
                        break;

                    case Goal.Download:
                        onDownlading.SetResult(true);
                        break;
                }
            };

            hooks.OnTaskCompleted += data =>
            {
                switch (data.Task)
                {
                    case Goal.Discover:
                        onDiscovered.SetResult(true);
                        break;

                    case Goal.Download:
                        onDownloaded.SetResult(true);
                        break;
                }
            };

            Downloading = onDownlading.Task;
            Downloaded = onDownloaded.Task;
            Discovering = onDiscovering.Task;
            Discovered = onDiscovering.Task;
        }
    }
}
