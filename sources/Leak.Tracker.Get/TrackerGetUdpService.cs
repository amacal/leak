using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Leak.Common;
using Leak.Networking.Core;
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
            buffer = new SocketBuffer(1556);
            socket = factory.Udp();

            context.Queue.Add(() =>
            {
                socket.Bind();
                socket.Receive(buffer, OnReceived);
            });
        }

        public void Stop()
        {
            collection.Clear();
            socket?.Dispose();
            socket = null;
        }

        public void Register(TrackerGetUdpRegistrant registrant)
        {
            byte[] transaction = Bytes.Random(4);
            TrackerGetUdpEntry entry = collection.Add(transaction);

            entry.Host = registrant.Host;
            entry.Port = registrant.Port;

            entry.Request = registrant.Request;
            entry.Callback = registrant.Callback;

            entry.Address = new Uri($"udp://{entry.Host}:{entry.Port}");
            entry.Deadline = DateTime.Now + TimeSpan.FromSeconds(context.Configuration.Timeout);
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
            context.CallTimeout(entry.Address, entry.Request.Hash);
        }

        private void ResolveHost(TrackerGetUdpEntry entry)
        {
            try
            {
                entry.Status = TrackerGetUdpStatus.Resolving;

                IPHostEntry found = Dns.GetHostEntry(entry.Host);
                IPAddress address = found.AddressList.FirstOrDefault();

                entry.Endpoint = new IPEndPoint(address, entry.Port);
                entry.Status = TrackerGetUdpStatus.Resolved;
            }
            catch (Exception ex)
            {
                context.CallFailed(entry.Address, entry.Request.Hash, ex.Message);
            }
        }

        private void SendConnectionRequest(TrackerGetUdpEntry entry)
        {
            SocketBuffer outgoing = new SocketBuffer(16);

            Array.Copy(TrackerGetUdpProtocol.Id, 0, outgoing.Data, 0, 8);
            Array.Copy(TrackerGetUdpProtocol.Connect, 0, outgoing.Data, 8, 4);
            Array.Copy(entry.Transaction, 0, outgoing.Data, 12, 4);

            entry.Status = TrackerGetUdpStatus.Connecting;
            socket.Send(entry.Endpoint, outgoing, OnSent(entry));
        }

        private void HandleConnectionResponse(IPEndPoint endpoint, TrackerGetUdpEntry entry, byte[] data)
        {
            if (data.Length < 16)
            {
                context.CallPacketIgnored(endpoint, data.Length);
                return;
            }

            if (Bytes.Equals(TrackerGetUdpProtocol.Connect, data, 0, 4) == false)
            {
                context.CallPacketIgnored(endpoint, data.Length);
                return;
            }

            entry.Connection = Bytes.Copy(data, 8, 8);
            entry.Status = TrackerGetUdpStatus.Connected;

            context.CallConnected(entry.Address, entry.Request.Hash, entry.Transaction, entry.Connection);
        }

        private void SendAnnounceRequest(TrackerGetUdpEntry entry)
        {
            SocketBuffer outgoing = new SocketBuffer(98);
            PeerHash peer = context.Configuration.Peer;

            outgoing[67] = 0x01;
            outgoing[83] = 0x02;
            outgoing[95] = 0xff;
            outgoing[96] = 0x1f;
            outgoing[97] = 0x90;

            Array.Copy(entry.Connection, 0, outgoing.Data, 0, 8);
            Array.Copy(TrackerGetUdpProtocol.Announce, 0, outgoing.Data, 8, 4);
            Array.Copy(entry.Transaction, 0, outgoing.Data, 12, 4);
            Array.Copy(entry.Request.Hash.ToBytes(), 0, outgoing.Data, 16, 20);
            Array.Copy(peer.ToBytes(), 0, outgoing.Data, 36, 20);

            entry.Status = TrackerGetUdpStatus.Announcing;
            socket.Send(entry.Endpoint, outgoing, OnSent(entry));
        }

        private void HandleAnnounceResponse(IPEndPoint endpoint, TrackerGetUdpEntry entry, byte[] data)
        {
            if (data.Length < 20)
            {
                context.CallPacketIgnored(endpoint, data.Length);
                return;
            }

            if (Bytes.Equals(TrackerGetUdpProtocol.Announce, data, 0, 4) == false)
            {
                context.CallPacketIgnored(endpoint, data.Length);
                return;
            }

            List<NetworkAddress> peers = new List<NetworkAddress>();

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
                    peers.Add(new NetworkAddress(address.ToString(), port));
                }
            }

            collection.Remove(entry.Transaction);
            entry.Callback.Invoke(interval);
            context.CallAnnounced(entry.Address, entry.Request.Hash, interval, seeders, leechers, peers.ToArray());
        }

        private void HandleErrorResponse(IPEndPoint endpoint, TrackerGetUdpEntry entry, byte[] data)
        {
            if (data.Length < 8)
            {
                context.CallPacketIgnored(endpoint, data.Length);
                return;
            }

            byte[] message = Bytes.Copy(data, 8);
            string reason = Encoding.ASCII.GetString(message);

            context.CallFailed(entry.Address, entry.Request.Hash, reason);
        }

        private UdpSocketSendCallback OnSent(TrackerGetUdpEntry entry)
        {
            return sent =>
            {
                if (sent.Status == SocketStatus.OK)
                {
                    int size = sent.Count;
                    IPEndPoint endpoint = sent.Endpoint;

                    context.Queue.Add(() =>
                    {
                        context.CallPacketSent(entry.Address, entry.Request.Hash, endpoint, size);
                    });
                }
            };
        }

        private void OnReceived(UdpSocketReceive received)
        {
            if (received.Status == SocketStatus.OK)
            {
                IPEndPoint endpoint = received.GetEndpoint();
                byte[] data = Bytes.Copy(buffer.Data, buffer.Offset, received.Count);

                socket.Receive(buffer, OnReceived);
                context.Queue.Add(OnReceived(endpoint, data));
            }
        }

        private Action OnReceived(IPEndPoint endpoint, byte[] data)
        {
            return () =>
            {
                context.CallPacketReceived(endpoint, data.Length);

                if (data.Length < 8)
                {
                    context.CallPacketIgnored(endpoint, data.Length);
                    return;
                }

                byte[] transaction = Bytes.Copy(data, 4, 4);
                TrackerGetUdpEntry entry = collection.Find(transaction);

                if (entry != null && Bytes.Equals(TrackerGetUdpProtocol.Error, data, 0, 4))
                {
                    HandleErrorResponse(endpoint, entry, data);
                    return;
                }

                switch (entry?.Status)
                {
                    case TrackerGetUdpStatus.Connecting:
                        HandleConnectionResponse(endpoint, entry, data);
                        break;

                    case TrackerGetUdpStatus.Announcing:
                        HandleAnnounceResponse(endpoint, entry, data);
                        break;

                    default:
                        context.CallPacketIgnored(endpoint, data.Length);
                        break;
                }
            };
        }
    }
}