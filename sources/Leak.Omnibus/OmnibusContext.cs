using Leak.Common;
using Leak.Data.Map.Components;
using Leak.Tasks;

namespace Leak.Data.Map
{
    public class OmnibusContext
    {
        private readonly OmnibusParameters parameters;
        private readonly OmnibusDependencies dependencies;
        private readonly OmnibusHooks hooks;
        private readonly OmnibusConfiguration configuration;
        private readonly OmnibusReservationCollection reservations;

        private readonly OmnibusStateCollection states;
        private readonly OmnibusBitfieldCollection bitfields;

        private readonly LeakQueue<OmnibusContext> queue;

        private OmnibusCache cache;
        private OmnibusPieceCollection pieces;

        private Metainfo metainfo;
        private Bitfield bitfield;

        public OmnibusContext(OmnibusParameters parameters, OmnibusDependencies dependencies, OmnibusConfiguration configuration, OmnibusHooks hooks)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.hooks = hooks;
            this.configuration = configuration;

            reservations = new OmnibusReservationCollection(configuration.LeaseDuration);
            queue = new LeakQueue<OmnibusContext>(this);
            states = new OmnibusStateCollection();
            bitfields = new OmnibusBitfieldCollection();
        }

        public OmnibusHooks Hooks
        {
            get { return hooks; }
        }

        public OmnibusParameters Parameters
        {
            get { return parameters; }
        }

        public OmnibusDependencies Dependencies
        {
            get { return dependencies; }
        }

        public OmnibusConfiguration Configuration
        {
            get { return configuration; }
        }

        public OmnibusBitfieldCollection Bitfields
        {
            get { return bitfields; }
        }

        public OmnibusPieceCollection Pieces
        {
            get { return pieces; }
            set { pieces = value; }
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
            set { cache = value; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
            set { metainfo = value; }
        }

        public Bitfield Bitfield
        {
            get { return bitfield; }
            set { bitfield = value; }
        }

        public OmnibusStateCollection States
        {
            get { return states; }
        }
    }
}