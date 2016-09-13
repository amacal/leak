using Leak.Core.Common;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Battlefield
{
    public class BattlefieldService
    {
        private readonly BattlefieldContext context;

        public BattlefieldService(Action<BattlefieldConfiguration> configurer)
        {
            context = new BattlefieldContext(configurer);
        }

        public void Handle(PeerSession session, BitfieldMessage message)
        {
            Bitfield bitfield = null;
            BattlefieldEntry entry = null;

            lock (context.Synchronized)
            {
                bitfield = message.ToBitfield();
                entry = context.Collection.GetOrCreate(session);

                entry.Bitfield = bitfield;
            }

            context.Callback.OnBitfieldChanged(session, bitfield);
        }

        public void Handle(PeerSession session, HaveMessage message)
        {
            Bitfield bitfield = null;
            BattlefieldEntry entry = null;

            lock (context.Synchronized)
            {
                entry = context.Collection.GetOrCreate(session);
                bitfield = entry.Bitfield;

                if (bitfield != null)
                {
                    bitfield[message.Piece] = true;
                }
            }

            if (bitfield != null)
            {
                context.Callback.OnBitfieldChanged(session, bitfield);
            }
        }

        public bool Contains(PeerSession session)
        {
            lock (context.Synchronized)
            {
                return context.Collection.GetOrCreate(session).Bitfield != null;
            }
        }

        public Bitfield Get(PeerSession session)
        {
            lock (context.Synchronized)
            {
                return context.Collection.GetOrCreate(session).Bitfield;
            }
        }

        public void Remove(PeerSession session)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(session);
            }
        }
    }
}