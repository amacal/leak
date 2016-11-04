using Leak.Sockets;
using System;
using System.Diagnostics;
using System.Net;

namespace Leak.Echo
{
    public class EchoBenchmarkWorker
    {
        private readonly Stopwatch watch;
        private readonly TcpSocketFactory factory;
        private readonly IPEndPoint endpoint;

        private Stopwatch session;
        private TcpSocket socket;
        private TcpSocketBuffer outgoing;
        private TcpSocketBuffer incoming;

        private static long counter;
        private static long previous;

        public EchoBenchmarkWorker(Stopwatch watch, TcpSocketFactory factory, IPEndPoint endpoint, byte[] outgoing)
        {
            this.watch = watch;
            this.endpoint = endpoint;

            this.outgoing = outgoing;
            this.incoming = new byte[outgoing.Length];
            this.session = Stopwatch.StartNew();

            this.factory = factory;
            this.socket = factory.Create();
        }

        public void Start()
        {
            socket.Bind();
            socket.Connect(endpoint, OnConnected);
        }

        private void OnConnected(TcpSocketConnect data)
        {
            if (data.Status == TcpSocketStatus.OK)
            {
                socket.Send(outgoing, OnSent);
            }
            else
            {
                Console.WriteLine("OnConnected");
                socket.Dispose();
            }
        }

        private void OnSent(TcpSocketSend data)
        {
            if (data.Status == TcpSocketStatus.OK)
            {
                outgoing = new TcpSocketBuffer(data.Buffer.Data, data.Buffer.Offset + data.Count, data.Buffer.Count - data.Count);

                if (incoming.Count > 0)
                {
                    socket.Receive(incoming, OnReceived);
                }
                else if (outgoing.Count > 0)
                {
                    socket.Send(outgoing, OnSent);
                }
                else
                {
                    incoming = new TcpSocketBuffer(incoming.Data);
                    outgoing = new TcpSocketBuffer(outgoing.Data);

                    if (session.Elapsed > TimeSpan.FromMinutes(1))
                    {
                        socket = factory.Create();
                        session = Stopwatch.StartNew();

                        socket.Bind();
                        socket.Connect(endpoint, OnConnected);
                        data.Socket.Dispose();
                    }
                    else
                    {
                        socket.Send(outgoing, OnSent);
                    }
                }
            }
            else
            {
                Console.WriteLine("OnSent");
                socket.Dispose();
            }

            counter += data.Count;

            if (counter - previous > 1024 * 1024 * 1024)
            {
                previous = counter;

                Console.WriteLine($"{counter}: {counter / watch.ElapsedMilliseconds}");
            }
        }

        private void OnReceived(TcpSocketReceive data)
        {
            if (data.Status == TcpSocketStatus.OK)
            {
                incoming = new TcpSocketBuffer(data.Buffer.Data, data.Buffer.Offset + data.Count, data.Buffer.Count - data.Count);

                if (outgoing.Count > 0)
                {
                    socket.Send(outgoing, OnSent);
                }
                else if (incoming.Count > 0)
                {
                    socket.Receive(incoming, OnReceived);
                }
                else
                {
                    incoming = new TcpSocketBuffer(incoming.Data);
                    outgoing = new TcpSocketBuffer(outgoing.Data);

                    if (session.Elapsed > TimeSpan.FromMinutes(1))
                    {
                        socket = factory.Create();
                        session = Stopwatch.StartNew();

                        socket.Bind();
                        socket.Connect(endpoint, OnConnected);
                        data.Socket.Dispose();
                    }
                    else
                    {
                        socket.Send(outgoing, OnSent);
                    }
                }
            }
            else
            {
                Console.WriteLine("OnReceived");
                socket.Dispose();
            }

            counter += data.Count;

            if (counter - previous > 1024 * 1024 * 1024)
            {
                previous = counter;

                Console.WriteLine($"{counter}: {counter / watch.ElapsedMilliseconds}");
            }
        }
    }
}