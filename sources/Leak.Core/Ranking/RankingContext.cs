using System;

namespace Leak.Core.Ranking
{
    public class RankingContext
    {
        private readonly object synchronized;
        private readonly RankingConfiguration configuration;
        private readonly RankingCollection collection;

        public RankingContext(Action<RankingConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Minimum = 0;
                with.Maximum = 8192;
            });

            synchronized = new object();
            collection = new RankingCollection();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public RankingConfiguration Configuration
        {
            get { return configuration; }
        }

        public RankingCollection Collection
        {
            get { return collection; }
        }
    }
}