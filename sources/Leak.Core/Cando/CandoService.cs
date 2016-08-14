using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Cando
{
    public class CandoService
    {
        private readonly CandoContext context;

        public CandoService(Action<CandoConfiguration> configurer)
        {
            context = new CandoContext(configurer);
        }

        public void Register(PeerConnectorHandshake handshake)
        {
            lock (context.Synchronized)
            {
                PeerHash peer = handshake.Peer;
                CandoEntry entry = context.Collection.GetOrCreate(peer);

                entry.HasExtensions = handshake.HasExtensions;
                entry.Direction = PeerDirection.Outgoing;

                entry.Handlers = context.Configuration.Extensions.ToHandlers();
                entry.Local = context.Configuration.Extensions.ToMap();
            }
        }

        public void Register(PeerListenerHandshake handshake)
        {
            lock (context.Synchronized)
            {
                PeerHash peer = handshake.Peer;
                CandoEntry entry = context.Collection.GetOrCreate(peer);

                entry.HasExtensions = handshake.HasExtensions;
                entry.Direction = PeerDirection.Incoming;

                entry.Handlers = context.Configuration.Extensions.ToHandlers();
                entry.Local = context.Configuration.Extensions.ToMap();
            }
        }

        public void Start(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                CandoEntry entry = context.Collection.GetOrCreate(peer);
                PeerDirection direction = entry.Direction;

                if (direction == PeerDirection.Outgoing)
                {
                    CallHandshakeIfRequired(entry);
                }
            }
        }

        public void Handle(PeerHash peer, ExtendedIncomingMessage message)
        {
            lock (context.Synchronized)
            {
                CandoEntry entry = context.Collection.GetOrCreate(peer);
                Extended payload = new Extended(message.Id, message.ToBytes());

                if (entry.HasExtensions)
                {
                    SendHandshakeIfRequested(entry, payload);
                    CallExtensionIfRequested(entry, payload);
                }
            }
        }

        public void Send(PeerHash peer, Func<CandoFormatter, Extended> callback)
        {
            lock (context.Synchronized)
            {
                CandoEntry entry = context.Collection.GetOrCreate(peer);
                CandoFormatter formatter = new CandoFormatter(entry.Remote);

                Extended payload = callback.Invoke(formatter);
                ExtendedOutgoingMessage message = new ExtendedOutgoingMessage(payload);

                context.Callback.OnOutgoingMessage(peer, message);
            }
        }

        public bool Supports(PeerHash peer, Func<CandoFormatter, bool> callback)
        {
            lock (context.Synchronized)
            {
                CandoEntry entry = context.Collection.GetOrCreate(peer);
                CandoFormatter formatter = new CandoFormatter(entry.Remote);

                return callback.Invoke(formatter);
            }
        }

        public void Remove(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(peer);
            }
        }

        private void SendHandshakeIfRequested(CandoEntry entry, Extended payload)
        {
            if (payload.Id == 0)
            {
                byte[] data = payload.Data;
                BencodedValue bencoded = Bencoder.Decode(data);

                entry.Remote = CandoMap.Parse(bencoded);
                entry.KnowsRemoteExtensions = true;

                CallHandshakeIfRequired(entry);
            }
        }

        private void CallExtensionIfRequested(CandoEntry entry, Extended payload)
        {
            if (payload.Id > 0 && entry.KnowsLocalExtensions)
            {
                string extension = entry.Local.Translate(payload.Id);
                CandoHandler handler = entry.Handlers.Find(extension);

                handler?.Handle(entry.Peer, payload);
            }
        }

        private void CallHandshakeIfRequired(CandoEntry entry)
        {
            if (entry.KnowsLocalExtensions == false && entry.HasExtensions)
            {
                BencodedValue bencoded = entry.Local.ToBencoded();
                byte[] data = Bencoder.Encode(bencoded);

                Extended extended = new Extended(0, data);
                ExtendedOutgoingMessage message = new ExtendedOutgoingMessage(extended);

                context.Callback.OnOutgoingMessage(entry.Peer, message);
                entry.KnowsLocalExtensions = true;
            }
        }
    }
}