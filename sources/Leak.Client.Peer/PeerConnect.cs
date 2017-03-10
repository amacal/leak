using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Peer
{
    public class PeerConnect
    {
        private readonly PeerSession session;

        public PeerConnect(PeerSession session)
        {
            this.session = session;
        }

        public PeerHash Peer
        {
            get { return session.Peer; }
        }

        public Task<PeerNotification> Next()
        {
            return session.Notifications.Next();
        }
    }
}