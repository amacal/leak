using Leak.Core.Common;
using Leak.Core.Network;
using System;

namespace Leak.Core.Bouncer
{
    /// <summary>
    /// Controls connections, remotes and peers. Knows the correlations between
    /// them and all statistics regarding them. Prevents them from being accepted
    /// and passed to other upper components in the stack.
    /// </summary>
    public class PeerBouncerService
    {
        private readonly PeerBouncerContext context;

        public PeerBouncerService(Action<PeerBouncerConfiguration> configurer)
        {
            context = new PeerBouncerContext(configurer);
        }

        public int Count()
        {
            lock (context.Synchronized)
            {
                return context.Collection.Count(x => x.Released == false);
            }
        }

        public bool Accept(NetworkConnectionInfo connection)
        {
            lock (context.Synchronized)
            {
                PeerAddress remote = connection.Remote;
                int count = context.Collection.Count(x => x.Released == false);

                bool canAcceptMoreConnections = count < context.Configuration.Connections;
                bool canAcceptRemote = context.Collection.FindOrDefaultByRemote(remote.ToString()) == null;

                return canAcceptMoreConnections && canAcceptRemote;
            }
        }

        public bool AcceptRemote(NetworkConnection connection)
        {
            lock (context.Synchronized)
            {
                long identifier = connection.Identifier;
                string remote = connection.Remote.ToString();

                PeerBouncerEntry byIdentifier = context.Collection.FindOrCreateByIdentifier(identifier);
                PeerBouncerEntry byRemote = context.Collection.FindOrDefaultByRemote(remote);
                int count = context.Collection.Count(x => x.Released == false);

                if (byIdentifier.Released)
                    return false;

                if (byIdentifier.Identifiers.Count == 0)
                    return false;

                if (byRemote != null)
                    return false;

                if (count > context.Configuration.Connections)
                    return false;

                byIdentifier.Remotes.Add(remote);
                context.Collection.AddByRemote(remote, byIdentifier);
            }

            return true;
        }

        public bool AcceptPeer(NetworkConnection connection, PeerHash peer)
        {
            lock (context.Synchronized)
            {
                long identifier = connection.Identifier;

                PeerBouncerEntry byIdentifier = context.Collection.FindOrCreateByIdentifier(identifier);
                PeerBouncerEntry byPeer = context.Collection.FindOrDefaultByPeer(peer);
                int count = context.Collection.Count(x => x.Released == false);

                if (byIdentifier.Released)
                    return false;

                if (byIdentifier.Identifiers.Count == 0)
                    return false;

                if (byIdentifier.Remotes.Count == 0)
                    return false;

                if (byPeer != null)
                    return false;

                if (count > context.Configuration.Connections)
                    return false;

                byIdentifier.Peers.Add(peer);
                context.Collection.AddByPeer(peer, byIdentifier);
            }

            return true;
        }

        public void AttachConnection(NetworkConnection connection)
        {
            lock (context.Synchronized)
            {
                long identifier = connection.Identifier;
                PeerBouncerEntry entry = context.Collection.FindOrCreateByIdentifier(identifier);

                if (entry.Released)
                    return;

                if (entry.Identifiers.Count > 0)
                    return;

                entry.Identifiers.Add(identifier);
            }
        }

        public void ReleaseConnection(NetworkConnection connection)
        {
            lock (context.Synchronized)
            {
                long identifier = connection.Identifier;
                PeerBouncerEntry entry = context.Collection.FindOrCreateByIdentifier(identifier);

                entry.Released = true;
            }
        }
    }
}