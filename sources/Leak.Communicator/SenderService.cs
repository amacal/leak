using Leak.Common;
using Leak.Networking.Core;
using Leak.Peer.Sender.Core;

namespace Leak.Peer.Sender
{
    public class SenderService
    {
        private readonly SenderContext context;

        public SenderService(SenderConfiguration configuration, SenderHooks hooks)
        {
            context = new SenderContext(configuration, hooks);
        }

        public SenderHooks Hooks
        {
            get { return context.Hooks; }
        }

        public void Add(PeerHash peer, NetworkConnection connection)
        {
            context.Collection.Add(peer, connection);
        }

        public void Remove(PeerHash peer)
        {
            context.Collection.Remove(peer);
        }

        public void SendKeepAlive(PeerHash peer)
        {
            NetworkConnection connection = context.Collection.Find(peer);

            if (connection != null)
            {
                connection.Send(new SenderKeepAlive());
                context.Hooks.CallKeepAliveSent(peer);
            }
        }

        public void Send(PeerHash peer, SenderMessage message)
        {
            NetworkConnection connection = context.Collection.Find(peer);
            SenderDefinition definition = context.Configuration.Definition;

            if (connection == null)
            {
                context.Hooks.CallMessageIgnored(peer, message.Type, message);
                return;
            }

            string type = message.Type;
            byte? identifier = definition?.GetIdentifier(type);

            if (identifier != null)
            {
                connection.Send(message.Apply(identifier.Value));
                context.Hooks.CallMessageSent(peer, type, message);
            }
            else
            {
                context.Hooks.CallMessageIgnored(peer, type, message);
                message.Release();
            }
        }
    }
}