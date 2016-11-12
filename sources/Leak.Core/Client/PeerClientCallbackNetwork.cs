using Leak.Core.Common;

namespace Leak.Core.Client
{
    public interface PeerClientCallbackNetwork
    {
        /// <summary>
        /// Called when the outgoing connection is being established.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="peer">The remote peer address.</param>
        void OnPeerConnectingTo(FileHash hash, PeerAddress peer);

        /// <summary>
        /// Called when the outgoing connection was successfully established.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="connected">Describes the current state.</param>
        void OnPeerConnectedTo(FileHash hash, PeerClientConnected connected);

        /// <summary>
        /// Called when the successfully connected peer was disconnected.
        /// </summary>
        /// <param name="hash">The hash of the affected resource.</param>
        /// <param name="peer">The disconnected remote peer.</param>
        void OnPeerDisconnected(FileHash hash, PeerHash peer);
    }
}