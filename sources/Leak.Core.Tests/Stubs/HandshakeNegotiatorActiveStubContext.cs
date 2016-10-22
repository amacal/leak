using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;
using System.Threading;

namespace Leak.Core.Tests.Stubs
{
    public class HandshakeNegotiatorActiveStubContext : HandshakeNegotiatorActiveContext
    {
        private readonly HandshakeNegotiatorActiveStubConfiguration configuration;
        private readonly ManualResetEvent onReady;

        private Handshake foundHandshake;
        private bool isClosed;

        public HandshakeNegotiatorActiveStubContext(Action<HandshakeNegotiatorActiveStubConfiguration> configurer)
        {
            this.configuration = new HandshakeNegotiatorActiveStubConfiguration
            {
                Hash = new FileHash(Bytes.Random(20)),
                Peer = new PeerHash(Bytes.Random(20))
            };

            configurer.Invoke(this.configuration);
            this.onReady = new ManualResetEvent(false);
        }

        public Handshake Handshake
        {
            get { return foundHandshake; }
        }

        public bool IsClosed
        {
            get { return isClosed; }
        }

        public WaitHandle Ready
        {
            get { return onReady; }
        }

        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        public FileHash Hash
        {
            get { return configuration.Hash; }
        }

        public HandshakeOptions Options
        {
            get { return HandshakeOptions.None; }
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
            foundHandshake = handshake;
            onReady.Set();
        }

        public void OnException(Exception ex)
        {
            isClosed = true;
            onReady.Set();
        }

        public void OnDisconnected()
        {
            isClosed = true;
            onReady.Set();
        }
    }
}