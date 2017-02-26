using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Leak.Common;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetUdpService
    {
        private readonly TrackerGetContext context;
        private readonly SocketFactory factory;
        private readonly TrackerGetUdpCollection collection;

        private SocketBuffer buffer;
        private UdpSocket socket;

        public TrackerGetUdpService(TrackerGetContext context)
        {
            this.context = context;

            factory = new SocketFactory(context.Dependencies.Worker);
            collection = new TrackerGetUdpCollection();
        }

        public void Start()
        {
            buffer = new SocketBuffer(4096);
            socket = factory.Udp();

            socket.Bind();
            socket.Receive(buffer, OnReceived);
        }

        public void Stop()
        {
            socket.Dispose();
            socket = null;
        }

        public void Register(byte[] transaction, TrackerGetUdpRegistrant registrant)
        {
            TrackerGetUdpEntry entry = collection.Add(transaction);

            entry.Hash = registrant.Hash;
            entry.Host = registrant.Host;
            entry.Port = registrant.Port;

            entry.Address = new Uri($"udp://{entry.Host}:{entry.Port}");
            entry.Deadline = DateTime.Now + context.Configuration.Timeout;
        }

        public void Schedule()
        {
            DateTime now = DateTime.Now;

            foreach (TrackerGetUdpEntry entry in collection.All())
            {
                if (entry.Deadline < now)
                {
                    HandleDeadline(entry);
                    continue;
                }

                switch (entry.Status)
                {
                    case TrackerGetUdpStatus.Pending:
                        ResolveHost(entry);
                        break;

                    case TrackerGetUdpStatus.Resolved:
                        SendConnectionRequest(entry);
                        break;

                    case TrackerGetUdpStatus.Connected:
                        SendAnnounceRequest(entry);
                        break;
                }
            }
        }

        private void HandleDeadline(TrackerGetUdpEntry entry)
        {
            collection.Remove(entry.Transaction);
            context.CallTrackerFailed(entry.Address, entry.Hash, "timeout");
        }

        private void ResolveHost(TrackerGetUdpEntry entry)
        {
            IPHostEntry found = Dns.GetHostEntry(entry.Host);
            IPAddress address = found.AddressList.FirstOrDefault();

            entry.Endpoint = new IPEndPoint(address, entry.Port);
            entry.Status = TrackerGetUdpStatus.Resolved;
        }

        private void SendConnectionRequest(TrackerGetUdpEntry entry)
        {
            SocketBuffer outgoing = new SocketBuffer(16);

            outgoing[02] = 0x04;
            outgoing[03] = 0x17;
            outgoing[04] = 0x27;
            outgoing[05] = 0x10;
            outgoing[06] = 0x19;
            outgoing[07] = 0x80;

            for (int i = 0; i < 4; i++)
            {
                outgoing[12 + i] = entry.Transaction[i];
            }

            entry.Status = TrackerGetUdpStatus.Connecting;
            socket.Send(entry.Endpoint, outgoing, OnSent);
        }

        private void HandleConnectionResponse(TrackerGetUdpEntry entry, byte[] data)
        {
            if (data.Length >= 16)
            {
                entry.Connection = Bytes.Copy(data, 8, 8);
                entry.Status = TrackerGetUdpStatus.Connected;
            }
        }

        private void SendAnnounceRequest(TrackerGetUdpEntry entry)
        {
            SocketBuffer outgoing = new SocketBuffer(98);

            outgoing[11] = 0x01;
            outgoing[67] = 0x01;
            outgoing[83] = 0x02;
            outgoing[95] = 0xff;
            outgoing[96] = 0x1f;
            outgoing[97] = 0x90;

            for (int i = 0; i < 8; i++)
            {
                outgoing[i] = entry.Connection[i];
            }

            for (int i = 0; i < 4; i++)
            {
                outgoing[12 + i] = entry.Transaction[i];
            }

            for (int i = 0; i < 20; i++)
            {
                outgoing[16 + i] = entry.Hash[i];
            }

            for (int i = 0; i < 20; i++)
            {
                outgoing[36 + 1] = context.Configuration.Peer[i];
            }

            entry.Status = TrackerGetUdpStatus.Announcing;
            socket.Send(entry.Endpoint, outgoing, OnSent);
        }

        private void HandleAnnounceResponse(TrackerGetUdpEntry entry, byte[] data)
        {
            if (data.Length >= 20)
            {
                List<PeerAddress> peers = new List<PeerAddress>();

                int intervalInSeconds = Bytes.ReadInt32(data, 8);
                TimeSpan interval = TimeSpan.FromSeconds(intervalInSeconds);

                int leechers = Bytes.ReadInt32(data, 12);
                int seeders = Bytes.ReadInt32(data, 16);

                for (int i = 20; i + 6 <= data.Length; i += 6)
                {
                    int port = Bytes.ReadUInt16(data, i + 4);
                    StringBuilder address = new StringBuilder();

                    address.Append(data[i].ToString());
                    address.Append('.');
                    address.Append(data[i + 1].ToString());
                    address.Append('.');
                    address.Append(data[i + 2].ToString());
                    address.Append('.');
                    address.Append(data[i + 3].ToString());

                    if (port > 0)
                    {
                        peers.Add(new PeerAddress(address.ToString(), port));
                    }
                }

                context.CallTrackerAnnounced(entry.Address, entry.Hash, interval, seeders, leechers, peers.ToArray());
            }
        }

        private void OnSent(UdpSocketSend sent)
        {
        }

        private void OnReceived(UdpSocketReceive received)
        {
            SocketStatus status = received.Status;
            byte[] data = Bytes.Copy(received.Buffer.Data, received.Buffer.Offset, received.Count);

            if (status == SocketStatus.OK)
            {
                socket.Receive(buffer, OnReceived);
                context.Queue.Add(OnReceived(data));
            }
        }

        private Action OnReceived(byte[] data)
        {
            return () =>
            {
                byte[] transaction = Bytes.Copy(data, 4, 4);
                TrackerGetUdpEntry entry = collection.Find(transaction);

                switch (entry?.Status)
                {
                    case TrackerGetUdpStatus.Connecting:
                        HandleConnectionResponse(entry, data);
                        break;

                    case TrackerGetUdpStatus.Announcing:
                        HandleAnnounceResponse(entry, data);
                        break;
                }
            };
        }
    }
}