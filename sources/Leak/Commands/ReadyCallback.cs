using Leak.Tasks;
using System.Threading;

namespace Leak.Commands
{
    public static class ReadyCallback
    {
        public static LeakBusCallback Complete(ManualResetEvent handle)
        {
            return (name, payload) =>
            {
                if (name == "file-completed")
                {
                    handle.Set();
                }
            };
        }
    }
}