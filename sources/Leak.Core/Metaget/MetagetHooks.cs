using System;
using Leak.Events;

namespace Leak.Core.Metaget
{
    public class MetagetHooks
    {
        /// <summary>
        /// Called when the metafile was the initially measured. It means
        /// that before the exact size of the metafile was not known.
        /// </summary>
        public Action<MetafileMeasured> OnMetafileMeasured;

        /// <summary>
        /// Called when the metadata piece was requested by asking remote
        /// peer to provide the piece.
        /// </summary>
        public Action<MetadataPieceRequested> OnMetadataPieceRequested;

        /// <summary>
        /// Called when the metadata piece was received from the remote
        /// peer. It does not mean that the received piece is consistent.
        /// The validity will be determined when all metadata pieces are
        /// downloaded and file hash matches against all metadata content.
        /// </summary>
        public Action<MetadataPieceReceived> OnMetadataPieceReceived;

        /// <summary>
        /// Called when the all pieces were received and the metadata was
        /// verified against the file-hash.
        /// </summary>
        public Action<MetadataDiscovered> OnMetadataDiscovered;
    }
}