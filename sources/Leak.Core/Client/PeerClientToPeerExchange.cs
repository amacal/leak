using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Extensions.PeerExchange;

namespace Leak.Core.Client
{
    public class PeerClientToPeerExchange : PeerExchangeCallbackBase
    {
        private readonly PeerClientExtensionContext context;

        public PeerClientToPeerExchange(PeerClientExtensionContext context)
        {
            this.context = context;
        }

        public override void OnMessage(PeerHash peer, PeerExchangeMessage message)
        {
            FileHash hash = context.GetHash(peer);
            PeerClientCallback callback = context.GetCallback(peer);
            PeerConnector connector = context.GetConnector(peer);

            foreach (PeerAddress added in message.Added)
            {
                if (context.IsConnected(added) == false)
                {
                    connector.ConnectTo(added);
                }
            }
        }
    }
}