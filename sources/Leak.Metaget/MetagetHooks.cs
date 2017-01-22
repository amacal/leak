using Leak.Events;
using System;

namespace Leak.Metaget
{
    public class MetagetHooks
    {
        /// <summary>
        /// Called when the metafile was the first time measured. It means
        /// that before the exact size of the metafile was not known.
        /// </summary>
        public Action<MetafileMeasured> OnMetafileMeasured;
    }
}