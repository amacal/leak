using System;
using System.Threading.Tasks;
using Leak.Client.Tracker.Exceptions;
using Leak.Common;
using Leak.Tracker.Get;
using Leak.Tracker.Get.Events;

namespace Leak.Client.Tracker
{
    public class TrackerClient : IDisposable
    {
        private readonly Uri address;
        private readonly TrackerRuntime runtime;
        private readonly TrackerCollection collection;
        private readonly TrackerLogger logger;

        public TrackerClient(Uri address)
        {
            this.address = address;
            this.runtime = new TrackerFactory(logger);
            this.collection = new TrackerCollection(logger);
        }

        public TrackerClient(Uri address, TrackerLogger logger)
        {
            this.address = address;
            this.logger = logger;

            this.runtime = new TrackerFactory(logger);
            this.collection = new TrackerCollection(logger);
        }

        public TrackerClient(Uri address, TrackerRuntime runtime)
        {
            this.address = address;
            this.runtime = runtime;

            this.collection = new TrackerCollection(logger);
        }

        public Task<TrackerAnnounce> AnnounceAsync(FileHash hash)
        {
            runtime.Start(new TrackerGetHooks
            {
                OnAnnounced = OnAnnounced,
                OnConnected = OnConnected,
                OnPacketSent = OnPacketSent,
                OnPacketReceived = OnPacketReceived,
                OnPacketIgnored = OnPacketIgnored,
                OnTimeout = OnTimeout,
                OnFailed = OnFailed
            });

            TaskCompletionSource<TrackerAnnounce> completion
                = new TaskCompletionSource<TrackerAnnounce>();

            TrackerEntry entry = collection.Add(hash);
            entry.Completion = completion;

            logger?.Info($"registering query for '{hash}'");
            runtime.Service.Register(address, hash);

            return completion.Task;
        }

        private void OnAnnounced(TrackerAnnounced data)
        {
            TrackerEntry entry = collection.Find(data.Hash);
            TrackerAnnounce announce = new TrackerAnnounce
            {
                Hash = data.Hash,
                Interval = data.Interval,
                Peers = data.Peers,
                Leechers = data.Leechers,
                Seeders = data.Seeders
            };

            logger?.Info($"announcing '{data.Hash}' completed; peers={data.Peers.Length}; leechers={data.Leechers}; seeds={data.Seeders}");
            entry?.Completion.TrySetResult(announce);

            collection.Remove(data.Hash);
        }

        private void OnConnected(TrackerConnected data)
        {
            string transaction = Bytes.ToString(data.Transaction);
            string connection = Bytes.ToString(data.Connection);

            logger?.Info($"announcing '{data.Hash}' in progress; status=connected; transaction={transaction}; connection={connection}");
        }

        private void OnPacketSent(TrackerPacketSent data)
        {
            string endpoint = data.Endpoint.ToString();
            string size = data.Size.ToString();

            logger?.Info($"packet sent; endpoint={endpoint}; size={size}");
        }

        private void OnPacketReceived(TrackerPacketReceived data)
        {
            string endpoint = data.Endpoint.ToString();
            string size = data.Size.ToString();

            logger?.Info($"packet received; endpoint={endpoint}; size={size}");
        }

        private void OnPacketIgnored(TrackerPacketIgnored data)
        {
            string endpoint = data.Endpoint.ToString();
            string size = data.Size.ToString();

            logger?.Info($"packet ignored; endpoint={endpoint}; size={size}");
        }

        private void OnTimeout(TrackerTimeout data)
        {
            logger?.Info($"announcing '{data.Hash}' reached a timeout; seconds={data.Seconds}");

            collection
                .Find(data.Hash)?.Completion
                .TrySetException(new TrackerTimeoutException(data.Hash, data.Seconds));

            collection.Remove(data.Hash);
        }

        private void OnFailed(TrackerFailed data)
        {
            logger?.Info($"announcing '{data.Hash}' failed; reason='{data.Reason}'");

            collection
                .Find(data.Hash)?.Completion
                .TrySetException(new TrackerFailedException(data.Hash, data.Reason));

            collection.Remove(data.Hash);
        }

        public void Dispose()
        {
            logger?.Info("disposing tracker client");
            runtime.Stop();
        }
    }
}