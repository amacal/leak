using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metadata;
using Leak.Core.Omnibus.Components;
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
        private readonly LeakQueue<OmnibusContext> queue;
        private readonly LeakTimer timer;

        public OmnibusContext(Action<OmnibusConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new OmnibusCallbackNothing();
                with.LeaseDuration = TimeSpan.FromSeconds(30);
            });

            bitfields = new OmnibusBitfieldCollection(configuration.Metainfo.Properties.Pieces);
            reservations = new OmnibusReservationCollection(configuration.LeaseDuration);

            queue = new LeakQueue<OmnibusContext>();
            timer = new LeakTimer(TimeSpan.FromMilliseconds(25));

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

        public LeakQueue<OmnibusContext> Queue
        {
            get { return queue; }
        }

        public LeakTimer Timer
        {
            get { return timer; }
        }
    }
}