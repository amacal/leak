using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Swarm
{
    public class SwarmSession : Session
    {
        private readonly SwarmConnect inner;

        internal SwarmSession(SwarmConnect inner)
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

        public void Seed(string destination)
        {
            inner.Seed(destination);
        }

        public Task<Notification> NextAsync()
        {
            return inner.Notifications.NextAsync();
        }
    }
}