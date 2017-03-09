using System;
using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Swarm
{
    public class SwarmClient : IDisposable
    {
        private readonly FileHash hash;
        private readonly SwarmRuntime runtime;

        public SwarmClient(FileHash hash)
        {
            this.hash = hash;
        }

        public void Download(string destination)
        {
        }

        public Task<SwarmNotification> Next()
        {
            return null;
        }

        public void Dispose()
        {
        }
    }
}