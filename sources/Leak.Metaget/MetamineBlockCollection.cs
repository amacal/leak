using System.Collections.Generic;

namespace Leak.Meta.Get
{
    public class MetamineBlockCollection
    {
        private readonly HashSet<MetamineBlock> blocks;

        public MetamineBlockCollection()
        {
            blocks = new HashSet<MetamineBlock>();
        }

        public bool Contains(MetamineBlock block)
        {
            return blocks.Contains(block);
        }

        public void Complete(MetamineBlock block)
        {
            blocks.Add(block);
        }
    }
}