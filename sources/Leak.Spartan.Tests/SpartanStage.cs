using Leak.Common;
using System.Threading.Tasks;

namespace Leak.Spartan.Tests
{
    public class SpartanStage
    {
        public Task Discovering;
        public Task Discovered;

        public Task Verifying;
        public Task Verified;

        public Task Downloading;
        public Task Downloaded;

        public SpartanStage(SpartanHooks hooks)
        {
            TaskCompletionSource<bool> onDiscovering = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onDiscovered = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onVerifying = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onVerified = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onDownlading = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onDownloaded = new TaskCompletionSource<bool>();

            hooks.OnTaskStarted += data =>
            {
                switch (data.Task)
                {
                    case Goal.Discover:
                        onDiscovering.SetResult(true);
                        break;

                    case Goal.Verify:
                        onVerifying.SetResult(true);
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

                    case Goal.Verify:
                        onVerified.SetResult(true);
                        break;

                    case Goal.Download:
                        onDownloaded.SetResult(true);
                        break;
                }
            };

            Downloading = onDownlading.Task;
            Downloaded = onDownloaded.Task;
            Verifying = onVerifying.Task;
            Verified = onVerified.Task;
            Discovering = onDiscovering.Task;
            Discovered = onDiscovering.Task;
        }
    }
}