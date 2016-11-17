using Leak.Core.Cando.PeerExchange;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Cando
{
    public class CandoBuilder
    {
        private readonly Dictionary<string, CandoHandler> items;

        public CandoBuilder()
        {
            items = new Dictionary<string, CandoHandler>();
        }

        public void PeerExchange(Action<PeerExchangeConfiguration> configurer)
        {
            items.Add("ut_pex", new PeerExchangeHandler(configurer));
        }

        public CandoMap ToMap()
        {
            byte index = 1;
            CandoMap result = new CandoMap();

            foreach (string name in items.Keys)
            {
                result.Add(index++, name);
            }

            return result;
        }

        public CandoHandler[] ToHandlers()
        {
            return items.Values.ToArray();
        }
    }
}