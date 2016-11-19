using Leak.Core.Events;
using System;

namespace Leak.Core.Spartan
{
    public class SpartanHooks
    {
        public Action<TaskStarted> OnTaskStarted;

        public Action<TaskCompleted> OnTaskCompleted;

        public Action<MetafileMeasured> OnMetadataMeasured;

        public Action<MetadataPieceRequested> OnMetadataPieceRequested;

        public Action<MetadataPieceReceived> OnMetadataPieceReceived;

        public Action<MetadataDiscovered> OnMetadataDiscovered;
    }
}