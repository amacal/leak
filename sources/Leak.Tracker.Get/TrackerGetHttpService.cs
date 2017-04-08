using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Leak.Bencoding;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Sockets;
using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetHttpService
    {
        private readonly TrackerGetContext context;
        private readonly SocketFactory factory;
        private readonly TrackerGetHttpCollection collection;

        public TrackerGetHttpService(TrackerGetContext context)
        {
            this.context = context;

            factory = new SocketFactory(context.Dependencies.Worker);
            collection = new TrackerGetHttpCollection();
        }

        public void Start()
        {
        }

        public void Stop()
        {
            foreach (TrackerGetHttpEntry entry in collection.All())
            {
                entry.Socket?.Dispose();
                entry.Socket = null;
            }

            collection.Clear();
        }

        public void Register(TrackerGetHttpRegistrant registrant)
        {
            TcpSocket socket = factory.Tcp();
            TrackerGetHttpEntry entry = collection.Add(socket);

            entry.Request = registrant.Request;
            entry.Address = registrant.Address;

            entry.Callback = registrant.Callback;
            entry.Deadline = DateTime.Now + TimeSpan.FromSeconds(context.Configuration.Timeout);
        }

        public void Schedule()
        {
            DateTime now = DateTime.Now;

            foreach (TrackerGetHttpEntry entry in collection.All())
            {
                if (entry.Deadline < now)
                {
                    HandleDeadline(entry);
                    continue;
                }

                switch (entry.Status)
                {
                    case TrackerGetHttpStatus.Pending:
                        ResolveHost(entry);
                        break;

                    case TrackerGetHttpStatus.Resolved:
                        ConnectToHost(entry);
                        break;

                    case TrackerGetHttpStatus.Connected:
                        SendAnnounceRequest(entry);
                        break;
                }
            }
        }

        private void HandleDeadline(TrackerGetHttpEntry entry)
        {
            collection.Remove(entry.Socket);
            context.CallTimeout(entry.Address, entry.Request.Hash);
        }

        private void ResolveHost(TrackerGetHttpEntry entry)
        {
            try
            {
                entry.Status = TrackerGetHttpStatus.Resolving;

                IPHostEntry found = Dns.GetHostEntry(entry.Address.Host);
                IPAddress address = found.AddressList.FirstOrDefault();

                entry.Endpoint = new IPEndPoint(address, entry.Address.Port);
                entry.Status = TrackerGetHttpStatus.Resolved;
            }
            catch (Exception ex)
            {
                context.CallFailed(entry.Address, entry.Request.Hash, ex.Message);
            }
        }

        private void ConnectToHost(TrackerGetHttpEntry entry)
        {
            entry.Status = TrackerGetHttpStatus.Connecting;
            entry.Buffer = new SocketBuffer(16384);

            entry.Socket.Bind();
            entry.Socket.Connect(entry.Endpoint, OnConnected(entry));
        }

        private void SendAnnounceRequest(TrackerGetHttpEntry entry)
        {
            StringBuilder builder = new StringBuilder(1024);
            string authority = entry.Address.Authority;
            string resource = entry.Address.GetComponents(UriComponents.Path, UriFormat.Unescaped);

            builder.Append("GET /");
            builder.Append(resource);

            builder.AppendHash(entry.Request.Hash);
            builder.AppendPeer(entry.Request.Peer ?? context.Configuration.Peer);

            builder.AppendPort(entry.Request);
            builder.AppendEvent(entry.Request);
            builder.AppendProgress(entry.Request);

            builder.Append("&compact=1&no_peer_id=1");

            builder.AppendLine(" HTTP/1.1");
            builder.Append("Host: ");
            builder.AppendLine(authority);
            builder.AppendLine();

            byte[] data = Encoding.ASCII.GetBytes(builder.ToString());
            SocketBuffer outgoing = new SocketBuffer(data);

            entry.Status = TrackerGetHttpStatus.Announcing;
            entry.Socket.Send(outgoing, OnSent(entry));
            entry.Socket.Receive(entry.Buffer, OnReceived(entry));
        }

        private TcpSocketConnectCallback OnConnected(TrackerGetHttpEntry entry)
        {
            return connected =>
            {
                if (connected.Status == SocketStatus.OK)
                {
                    Action callback = () =>
                    {
                        context.CallConnected(entry.Address, entry.Request.Hash);
                    };

                    entry.Status = TrackerGetHttpStatus.Connected;
                    context.Queue.Add(callback);
                }
            };
        }

        private TcpSocketSendCallback OnSent(TrackerGetHttpEntry entry)
        {
            return sent =>
            {
                if (sent.Status == SocketStatus.OK)
                {
                    context.Queue.Add(() =>
                    {
                        context.CallPacketSent(entry.Address, entry.Request.Hash, entry.Endpoint, sent.Count);
                    });
                }
            };
        }

        private TcpSocketReceiveCallback OnReceived(TrackerGetHttpEntry entry)
        {
            return received =>
            {
                if (received.Status == SocketStatus.OK)
                {
                    context.Queue.Add(() =>
                    {
                        byte[] data = Bytes.Copy(entry.Buffer.Data, entry.Buffer.Offset, received.Count);
                        string text = Encoding.ASCII.GetString(data);

                        context.CallPacketReceived(entry.Endpoint, data.Length);

                        if (text.StartsWith(TrackerGetHttpProtocol.ResponseHeader) == false)
                        {
                            context.CallPacketIgnored(entry.Endpoint, data.Length);
                            return;
                        }

                        int counter = 0, position = 0;
                        bool r = false, n = false;

                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i] == '\r')
                            {
                                r = true;
                                counter++;
                                continue;
                            }

                            if (data[i] == '\n')
                            {
                                n = true;
                                counter++;
                                continue;
                            }

                            if (counter == 4 && r && n)
                            {
                                position = i;
                                break;
                            }

                            if (counter == 2 && !(r && n))
                            {
                                position = i;
                                break;
                            }

                            counter = 0;
                        }

                        if (position == 0)
                        {
                            context.CallPacketIgnored(entry.Endpoint, data.Length);
                            return;
                        }

                        BencodedValue decoded = Bencoder.Decode(data, position);
                        if (decoded.Dictionary == null)
                        {
                            context.CallPacketIgnored(entry.Endpoint, data.Length);
                            return;
                        }

                        string failure = decoded.Find("failure reason", x => x?.Text?.GetString());
                        if (failure != null)
                        {
                            context.CallFailed(entry.Address, entry.Request.Hash, failure);
                            return;
                        }

                        int? interval = decoded.Find("interval", x => x?.ToInt32());
                        BencodedValue peers = decoded.Find("peers", x => x);

                        if (interval != null && peers.Text != null && peers.Text.Length % 6 == 0)
                        {
                            List<NetworkAddress> result = new List<NetworkAddress>(peers.Text.Length / 6);
                            byte[] bytes = peers.Data.GetBytes();

                            for (int i = 0; i < bytes.Length; i += 6)
                            {
                                int port = Bytes.ReadUInt16(bytes, i + 4);
                                StringBuilder address = new StringBuilder();

                                address.Append(bytes[i].ToString());
                                address.Append('.');
                                address.Append(bytes[i + 1].ToString());
                                address.Append('.');
                                address.Append(bytes[i + 2].ToString());
                                address.Append('.');
                                address.Append(bytes[i + 3].ToString());

                                if (port > 0)
                                {
                                    result.Add(new NetworkAddress(address.ToString(), port));
                                }
                            }

                            collection.Remove(entry.Socket);
                            entry.Callback.Invoke(TimeSpan.FromSeconds(interval.Value));
                            context.CallAnnounced(entry.Address, entry.Request.Hash, TimeSpan.FromSeconds(interval.Value), result.ToArray());
                        }
                    });
                }
            };
        }
    }
}