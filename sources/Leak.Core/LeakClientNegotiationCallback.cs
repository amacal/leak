using Leak.Core.Net;
using Leak.Core.Network;

namespace Leak.Core
{
    public class LeakClientNegotiationCallback : PeerNegotiatorCallback
    {
        private readonly LeakConfiguration configuration;
        private readonly LeakData data;

        public LeakClientNegotiationCallback(LeakConfiguration configuration, LeakData data)
        {
            this.configuration = configuration;
            this.data = data;
        }

        public void OnConnect(NetworkConnection connection)
        {
            data.Peers.Connect(connection);
        }

        public void OnTerminate(NetworkConnection connection)
        {
            data.Peers.Terminate(connection);
        }

        public void OnHandshake(NetworkConnection connection, PeerHandshake handshake)
        {
            LeakCallbackHandshakeExchanged payload = new LeakCallbackHandshakeExchanged
            {
                SupportsExtensions = handshake.Options.HasFlag(PeerHandshakeOptions.Extended)
            };

            configuration.Callback.OnHandshakeExchanged?.Invoke(payload);
            handshake.Accept(new LeakClientPeerCallback(configuration, data));
        }
    }
}