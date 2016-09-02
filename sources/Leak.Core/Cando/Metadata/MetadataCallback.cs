﻿using Leak.Core.Common;

namespace Leak.Core.Cando.Metadata
{
    public interface MetadataCallback
    {
        void OnSize(PeerSession session, MetadataSize size);

        void OnRequest(PeerSession session, MetadataRequest request);

        void OnData(PeerSession session, MetadataData data);

        void OnReject(PeerSession session, MetadataReject reject);
    }
}