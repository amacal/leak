using Leak.Core.Common;
using Leak.Core.Metadata;
using Leak.Core.Repository;
using Leak.Files;

namespace Leak.Core.Retriever
{
    public class RetrieverConfiguration
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public Bitfield Bitfield { get; set; }

        public FileFactory Files { get; set; }

        public RetrieverCallback Callback { get; set; }

        public RetrieverStrategy Strategy { get; set; }

        public RepositoryService Repository { get; set; }
    }
}