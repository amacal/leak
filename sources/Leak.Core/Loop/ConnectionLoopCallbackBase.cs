using System;

namespace Leak.Core.Loop
{
    public abstract class ConnectionLoopCallbackBase : ConnectionLoopCallback
    {
        public virtual void OnAttached(ConnectionLoopChannel channel)
        {
        }

        public virtual void OnKeepAlive(ConnectionLoopChannel channel)
        {
        }

        public virtual void OnChoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
        }

        public virtual void OnUnchoke(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
        }

        public virtual void OnInterested(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
        }

        public virtual void OnHave(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
        }

        public virtual void OnBitfield(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
        }

        public virtual void OnPiece(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
        }

        public virtual void OnExtended(ConnectionLoopChannel channel, ConnectionLoopMessage message)
        {
        }

        public virtual void OnException(ConnectionLoopChannel channel, Exception ex)
        {
        }

        public virtual void OnDisconnected(ConnectionLoopChannel channel)
        {
        }
    }
}