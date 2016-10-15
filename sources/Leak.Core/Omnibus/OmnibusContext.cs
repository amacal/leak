using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metadata;
using Leak.Core.Omnibus.Components;
using System;

namespace Leak.Core.Omnibus
{
    public class OmnibusContext
    {
        private readonly OmnibusConfiguration configuration;
        private readonly OmnibusCache cache;
        private readonly OmnibusBitfieldCollection bitfields;
        private readonly OmnibusPieceCollection pieces;
        private readonly OmnibusReservationCollection reservations;
        private readonly LeakQueue<OmnibusContext> queue;

        public OmnibusContext(Action<OmnibusConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new OmnibusCallbackNothing();
                with.LeaseDuration = TimeSpan.FromSeconds(30);
                with.SchedulerThreshold = 16;
            });

            cache = new OmnibusCache(configuration.Metainfo.Properties.Pieces);
            bitfields = new OmnibusBitfieldCollection(cache);
            reservations = new OmnibusReservationCollection(configuration.LeaseDuration);

            queue = new LeakQueue<OmnibusContext>(this);
            pieces = new OmnibusPieceCollection(this);
        }

        public int SchedulerThreshold
        {
            get { return configuration.SchedulerThreshold; }
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

        public OmnibusCache Cache
        {
            get { return cache; }
        }
    }
}