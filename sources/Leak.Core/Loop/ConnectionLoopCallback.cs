using System;

namespace Leak.Core.Loop
{
    public interface ConnectionLoopCallback
    {
        void OnConnected(ConnectionLoopChannel channel);

        void OnKeepAlive(ConnectionLoopChannel channel);

        void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message);

        void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message);

        void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message);

        void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message);

        void OnException(ConnectionLoopChannel channel, Exception ex);

        void OnDisconnected(ConnectionLoopChannel channel);
    }
}