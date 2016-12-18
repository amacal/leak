using Leak.Events;
using System;

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
        /// Called when the all pieces were received and the metadata was
        /// verified against the file-hash.
        /// </summary>
        public Action<MetadataDiscovered> OnMetadataDiscovered;
    }
}