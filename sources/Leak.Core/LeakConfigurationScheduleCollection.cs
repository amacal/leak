using Leak.Core.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core
{
    public class LeakConfigurationScheduleCollection : IEnumerable<LeakConfigurationSchedule>
    {
        private readonly List<LeakConfigurationSchedule> items;

        public LeakConfigurationScheduleCollection()
        {
            items = new List<LeakConfigurationSchedule>();
        }

        public void Add(LeakConfigurationSchedule schedule)
        {
            items.Add(schedule);
        }

        public PeerNegotiatorHashCollection ToHashCollection()
        {
            return new PeerNegotiatorHashCollection(items.Select(x => x.Hash).ToArray());
        }

        public byte[] ToHash()
        {
            return items.Select(x => x.Hash).Single();
        }

        public IEnumerator<LeakConfigurationSchedule> GetEnumerator()
        {
            return items.AsReadOnly().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}