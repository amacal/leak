using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Congestion;

namespace Leak.Core.Collector
{
    public abstract class PeerCollectorCriterion
    {
        public static readonly PeerCollectorCriterion IsLocalNotChokedByRemote = new IsLocalNotChokedByRemoteCriterion();

        public static readonly PeerCollectorCriterion DoesRemoteSupportMetadata = new DoesRemoteSupportMetadataCriterion();

        public abstract bool Accept(PeerHash peer, PeerCollectorContext context);

        private class IsLocalNotChokedByRemoteCriterion : PeerCollectorCriterion
        {
            public override bool Accept(PeerHash peer, PeerCollectorContext context)
            {
                return context.Congestion.IsChoking(peer, PeerCongestionDirection.Remote) == false;
            }
        }

        private class DoesRemoteSupportMetadataCriterion : PeerCollectorCriterion
        {
            public override bool Accept(PeerHash peer, PeerCollectorContext context)
            {
                return context.Cando.Supports(peer, formatter => formatter.DoesSupportMetadata());
            }
        }
    }
}