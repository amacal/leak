using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Swarm
{
    public class SwarmSession
    {
        private readonly SwarmConnect inner;

        public SwarmSession(SwarmConnect inner)
        {
            this.inner = inner;
        }

        public FileHash Hash
        {
            get { return inner.Hash; }
        }

        public void Download(string destination)
        {
            inner.Download(destination);
        }

        public Task<Notification> Next()
        {
            return inner.Notifications.Next();
        }
    }
}