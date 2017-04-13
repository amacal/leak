using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Receiver
{
    public class ReceiverService
    {
        private readonly ReceiverHooks hooks;
        private readonly ReceiverConfiguration configuration;

        public ReceiverService(ReceiverParameters parameters, ReceiverConfiguration configuration, ReceiverHooks hooks)
        {
            this.hooks = hooks;
            this.configuration = configuration;
        }

        public void StartProcessing(PeerHash peer, NetworkConnection connection)
        {
            ReceiverConnection wrapped = new ReceiverConnection(connection);
            ReceiverHandler handler = new ReceiverHandler(peer, wrapped, configuration, hooks);

            handler.Execute();
        }
    }
}