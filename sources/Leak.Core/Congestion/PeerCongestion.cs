using Leak.Core.Common;
using System;

namespace Leak.Core.Congestion
{
    /// <summary>
    /// Manages the current status of each connected or even not connected peer.
    /// Provides a mechanism to query collected data in a very simple and efficient way.
    /// </summary>
    public class PeerCongestion
    {
        private readonly object synchronized;
        private readonly PeerCongestionCollection collection;
        private readonly PeerCongestionConfiguration configuration;

        public PeerCongestion(Action<PeerCongestionConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerCongestionCallbackNothing();
            });

            synchronized = new object();
            collection = new PeerCongestionCollection();
        }

        public bool IsInterested(PeerHash peer, PeerCongestionDirection direction)
        {
            lock (synchronized)
            {
                return collection.GetOrCreate(peer).GetState(direction).IsInterested;
            }
        }

        public bool IsChoking(PeerHash peer, PeerCongestionDirection direction)
        {
            lock (synchronized)
            {
                return collection.GetOrCreate(peer).GetState(direction).IsChoking;
            }
        }

        public void SetInterested(PeerHash peer, PeerCongestionDirection direction, bool value)
        {
            lock (synchronized)
            {
                collection.GetOrCreate(peer).GetState(direction).IsInterested = value;
            }
        }

        public void SetChoking(PeerHash peer, PeerCongestionDirection direction, bool value)
        {
            lock (synchronized)
            {
                collection.GetOrCreate(peer).GetState(direction).IsChoking = value;
            }
        }
    }
}