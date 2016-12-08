using Leak.Common;
using Leak.Core.Omnibus.Components;
using Leak.Tasks;

namespace Leak.Core.Omnibus
{
    public class OmnibusContext
    {
        private readonly Metainfo metainfo;
        private readonly Bitfield bitfield;
        private readonly OmnibusHooks hooks;
        private readonly OmnibusConfiguration configuration;
        private readonly OmnibusCache cache;
        private readonly OmnibusBitfieldCollection bitfields;
        private readonly OmnibusPieceCollection pieces;
        private readonly OmnibusReservationCollection reservations;
        private readonly OmnibusStateCollection states;

        private readonly LeakQueue<OmnibusContext> queue;

        public OmnibusContext(Metainfo metainfo, Bitfield bitfield, OmnibusHooks hooks, OmnibusConfiguration configuration)
        {
            this.metainfo = metainfo;
            this.bitfield = bitfield;
            this.hooks = hooks;
            this.configuration = configuration;

            cache = new OmnibusCache(metainfo.Properties.Pieces);
            bitfields = new OmnibusBitfieldCollection(cache);
            reservations = new OmnibusReservationCollection(configuration.LeaseDuration);

            queue = new LeakQueue<OmnibusContext>(this);
            pieces = new OmnibusPieceCollection(this);
            states = new OmnibusStateCollection();
        }

        public int SchedulerThreshold
        {
            get { return configuration.SchedulerThreshold; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public Bitfield Bitfield
        {
            get { return bitfield; }
        }

        public OmnibusHooks Hooks
        {
            get { return hooks; }
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

        public OmnibusStateCollection States
        {
            get { return states; }
        }
    }
}