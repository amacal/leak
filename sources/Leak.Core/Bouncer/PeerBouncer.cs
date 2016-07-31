using Leak.Core.Common;
using Leak.Core.Network;
using System;

namespace Leak.Core.Bouncer
{
    public class PeerBouncer
    {
        private readonly PeerBouncerConfiguration configuration;
        private readonly PeerBouncerStorage storage;

        public PeerBouncer(Action<PeerBouncerConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerBouncerCallbackNothing();
            });

            storage = new PeerBouncerStorage(configuration);
        }

        public bool AcceptRemote(NetworkConnection connection)
        {
            if (storage.AddRemote(connection.Remote) == false)
            {
                connection.Terminate();
                return false;
            }

            return true;
        }

        public bool AcceptPeer(NetworkConnection connection, PeerHash peer)
        {
            if (storage.AddPeer(connection.Remote, peer) == false)
            {
                connection.Terminate();
                return false;
            }

            return true;
        }

        public void ReleaseRemote(NetworkConnection connection)
        {
            storage.RemoveRemote(connection.Remote);
        }
    }
}