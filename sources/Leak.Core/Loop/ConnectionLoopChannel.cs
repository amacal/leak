using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Network;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopChannel
    {
        private readonly ConnectionLoopConfiguration configuration;
        private readonly ConnectionLoopConnection connection;
        private readonly ConnectionLoopHandshake handshake;

        public ConnectionLoopChannel(ConnectionLoopConfiguration configuration, ConnectionLoopConnection connection, ConnectionLoopHandshake handshake)
        {
            this.configuration = configuration;
            this.connection = connection;
            this.handshake = handshake;
        }

        public PeerEndpoint Endpoint
        {
            get
            {
                PeerDirection direction = GetDirection(connection.Direction);
                PeerAddress address = PeerAddress.Parse(connection.Remote);
                PeerEndpoint endpoint = new PeerEndpoint(handshake.Session, address, direction);

                return endpoint;
            }
        }

        private static PeerDirection GetDirection(NetworkDirection direction)
        {
            return direction == NetworkDirection.Incoming ? PeerDirection.Incoming : PeerDirection.Outgoing;
        }

        public bool SupportsExtensions()
        {
            return handshake.HasExtensions;
        }

        public void Send(KeepAliveMessage message)
        {
            Forward(message);
        }

        public void Send(InterestedMessage message)
        {
            Forward(message);
        }

        public void Send(BitfieldMessage message)
        {
            Forward(message);
        }

        public void Send(params RequestOutgoingMessage[] messages)
        {
            Forward(messages);
        }

        public void Send(ExtendedOutgoingMessage message)
        {
            Forward(message);
        }

        private void Forward(NetworkOutgoingMessage message)
        {
            try
            {
                connection.Send(message);
            }
            catch (Exception ex)
            {
                configuration.Callback.OnException(this, ex);
            }
        }

        private void Forward(NetworkOutgoingMessage[] messages)
        {
            try
            {
                connection.Send(new ConnectionLoopCompositeMessage(messages));
            }
            catch (Exception ex)
            {
                configuration.Callback.OnException(this, ex);
            }
        }
    }
}