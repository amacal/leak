using Leak.Core.Client;
using Leak.Core.Common;
using System.Threading;

namespace Leak.Commands
{
    public class ReadyCallback : PeerClientCallbackBase
    {
        private readonly ManualResetEvent handle;

        public ReadyCallback(ManualResetEvent handle)
        {
            this.handle = handle;
        }

        public override void OnCompleted(FileHash hash)
        {
            handle.Set();
        }
    }
}