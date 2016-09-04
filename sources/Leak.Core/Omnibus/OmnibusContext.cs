using Leak.Core.Common;
using Leak.Core.Metadata;
using System;

namespace Leak.Core.Omnibus
{
    public class OmnibusContext
    {
        private readonly object synchronized;
        private readonly OmnibusConfiguration configuration;
        private readonly OmnibusBitfieldCollection bitfields;
        private readonly OmnibusPieceCollection pieces;
        private readonly OmnibusReservationCollection reservations;

        public OmnibusContext(Action<OmnibusConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new OmnibusCallbackNothing();
            });

            bitfields = new OmnibusBitfieldCollection(configuration.Metainfo.Properties.Pieces);
            reservations = new OmnibusReservationCollection();

            synchronized = new object();
            pieces = new OmnibusPieceCollection(this);
        }

        public Metainfo Metainfo
        {
            get { return configuration.Metainfo; }
        }

        public Bitfield Bitfield
        {
            get { return configuration.Bitfield; }
        }

        public OmnibusCallback Callback
        {
            get { return configuration.Callback; }
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public OmnibusBitfieldCollection Bitfields
        {
            get { return bitfields; }
        }

        public OmnibusPieceCollection Pieces
        {
            get { return pieces; }
        }

        public OmnibusReservationCollection Reservations
        {
            get { return reservations; }
        }
    }
}