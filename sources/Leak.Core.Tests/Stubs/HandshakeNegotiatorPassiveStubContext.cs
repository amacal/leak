using System;
using System.Collections.Generic;
using System.Threading;
using Leak.Core.Common;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Core.Tests.Negotiator;

namespace Leak.Core.Tests.Stubs
{
    public class HandshakeNegotiatorPassiveStubContext : HandshakeNegotiatorPassiveContext
    {
        private readonly HandshakeNegotiatorPassiveStubConfiguration configuration;
        private readonly ManualResetEvent onReady;

        private Handshake foundHandshake;
        private HandshakeRejection foundRejection;

        public HandshakeNegotiatorPassiveStubContext(Action<HandshakeNegotiatorPassiveStubConfiguration> configurer)
        {
            this.configuration = new HandshakeNegotiatorPassiveStubConfiguration
            {
                Hashes = new List<FileHash>(),
                Peer = new PeerHash(Bytes.Random(20))
            };

            configurer.Invoke(this.configuration);
            this.onReady = new ManualResetEvent(false);
        }

        public Handshake Handshake
        {
            get { return foundHandshake; }
        }

        public HandshakeRejection Rejection
        {
            get { return foundRejection; }
        }

        public WaitHandle Ready
        {
            get { return onReady; }
        }

        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        public HandshakeOptions Options
        {
            get { return HandshakeOptions.None; }
        }

        public FileHashCollection Hashes
        {
            get { return new FileHashCollection(configuration.Hashes.ToArray()); }
        }

        public void OnRejected(HandshakeRejection rejection)
        {
            foundRejection = rejection;
            onReady.Set();
        }

        public void OnHandshake(NetworkConnection negotiated, Handshake handshake)
        {
            foundHandshake = handshake;
            onReady.Set();
        }

        public void OnException(Exception ex)
        {
        }

        public void OnDisconnected()
        {
        }
    }
}