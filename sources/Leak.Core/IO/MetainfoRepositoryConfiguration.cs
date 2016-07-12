using System;
using System.Collections.Generic;

namespace Leak.Core.IO
{
    public class MetainfoRepositoryConfiguration
    {
        public MetainfoRepositoryConfiguration()
        {
            this.Includes = new List<MetainfoRepositoryInclude>();
        }

        public MetainfoRepositoryStorage Storage { get; set; }

        public List<MetainfoRepositoryInclude> Includes { get; set; }

        public Action<MetainfoFile> OnCompleted { get; set; }
    }
}