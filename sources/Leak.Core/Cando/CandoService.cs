using Leak.Core.Bencoding;
using Leak.Core.Cando.Events;
using Leak.Core.Messages;
using System;
using Leak.Common;

namespace Leak.Core.Cando
{
    public class CandoService
    {
        private readonly CandoContext context;

        public CandoService(Action<CandoConfiguration> configurer)
        {
            context = new CandoContext(configurer);
        }

        public void Handle(PeerSession session, ExtendedIncomingMessage message)
        {
            lock (context.Synchronized)
            {
                CandoEntry entry = context.Collection.GetOrCreate(session);
                Extended payload = new Extended(message.Id, message.ToBytes());

                if (entry.HasExtensions)
                {
                    SendHandshakeIfRequested(entry, payload);
                    CallExtensionIfRequested(entry, payload);
                }
            }
        }

        public void Send(PeerSession session, Func<CandoFormatter, Extended> callback)
        {
            lock (context.Synchronized)
            {
                CandoEntry entry = context.Collection.GetOrCreate(session);
                CandoFormatter formatter = new CandoFormatter(entry.Remote);

                Extended payload = callback.Invoke(formatter);
                ExtendedOutgoingMessage message = new ExtendedOutgoingMessage(payload);

                context.Callback.OnOutgoingMessage(session, message);
            }
        }

        public bool Supports(PeerSession session, Func<CandoFormatter, bool> callback)
        {
            lock (context.Synchronized)
            {
                CandoEntry entry = context.Collection.GetOrCreate(session);
                CandoFormatter formatter = new CandoFormatter(entry.Remote);

                return callback.Invoke(formatter);
            }
        }

        public void Remove(PeerSession session)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(session);
            }
        }

        private void SendHandshakeIfRequested(CandoEntry entry, Extended payload)
        {
            if (payload.Id == 0)
            {
                byte[] data = payload.Data;
                BencodedValue handshake = Bencoder.Decode(data);

                entry.Remote = CandoMap.Parse(handshake);
                entry.KnowsRemoteExtensions = true;

                CallHandshakeOnEachHandler(entry, handshake);
                CallHandshakeIfRequired(entry);

                context.Bus.Publish("extension-exchanged", new ExtensionExchanged
                {
                    Peer = entry.Session.Peer,
                    Extensions = entry.Remote.All()
                });
            }
        }

        private void CallHandshakeOnEachHandler(CandoEntry entry, BencodedValue handshake)
        {
            foreach (CandoHandler handler in entry.Handlers)
            {
                handler.OnHandshake(entry.Session, handshake);
            }
        }

        private void CallExtensionIfRequested(CandoEntry entry, Extended payload)
        {
            if (payload.Id > 0 && entry.KnowsLocalExtensions)
            {
                string extension = entry.Local.Translate(payload.Id);
                CandoHandler handler = entry.Handlers.Find(extension);

                handler?.OnMessage(entry.Session, payload);
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

                context.Callback.OnOutgoingMessage(entry.Session, message);
                entry.KnowsLocalExtensions = true;
            }
        }
    }
}