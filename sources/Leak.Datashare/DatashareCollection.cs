using Leak.Common;
using System.Collections.Generic;

namespace Leak.Datashare
{
    public class DatashareCollection
    {
        private readonly Dictionary<BlockIndex, List<DatashareEntry>> byBlocks;

        public DatashareCollection()
        {
            byBlocks = new Dictionary<BlockIndex, List<DatashareEntry>>();
        }

        public void Register(PeerHash peer, BlockIndex block)
        {
            List<DatashareEntry> entries;

            if (byBlocks.TryGetValue(block, out entries) == false)
            {
                entries = new List<DatashareEntry>();
                byBlocks.Add(block, entries);
            }

            entries.Add(new DatashareEntry
            {
                Peer = peer,
                Block = block
            });
        }

        public IList<DatashareEntry> RemoveAll(BlockIndex block)
        {
            List<DatashareEntry> entries;

            if (byBlocks.TryGetValue(block, out entries) == false)
            {
                entries = new List<DatashareEntry>();
            }

            return entries;
        }
    }
}