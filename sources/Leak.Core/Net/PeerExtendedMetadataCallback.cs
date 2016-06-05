using System;

namespace Leak.Core.Net
{
    public class PeerExtendedMetadataCallback
    {
        public Action<PeerChannel, PeerExtendedMetadataRequest> OnRequest { get; set; }

        public Action<PeerChannel, PeerExtendedMetadataData> OnData { get; set; }

        public Action<PeerChannel, PeerExtendedMetadataReject> OnReject { get; set; }
    }
}