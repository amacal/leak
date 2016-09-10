using Leak.Core.Collector;

namespace Leak.Core.Client.Configuration
{
    public class PeerClientMetadataBuilder
    {
        private PeerClientMetadataStatus status;

        public PeerClientMetadataBuilder()
        {
            status = PeerClientMetadataStatus.Off;
        }

        public void Disable()
        {
            status = PeerClientMetadataStatus.Off;
        }

        public void Enable()
        {
            status = PeerClientMetadataStatus.On;
        }

        public void Apply(PeerCollectorConfiguration configuration)
        {
            if (status == PeerClientMetadataStatus.On)
            {
                configuration.Extensions.IncludeMetadata();
            }
        }
    }
}