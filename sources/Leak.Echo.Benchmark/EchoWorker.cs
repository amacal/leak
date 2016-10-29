using Leak.Sockets;
using System;
using System.Diagnostics;
using System.Net;

namespace Leak.Echo.Benchmark
{
    public class EchoWorker
    {
        private readonly Stopwatch watch;
        private readonly TcpSocket socket;
        private readonly IPEndPoint endpoint;

        private TcpSocketBuffer outgoing;
        private TcpSocketBuffer incoming;

        private static long counter;
        private static long previous;

        public EchoWorker(Stopwatch watch, TcpSocket socket, IPEndPoint endpoint, byte[] outgoing)
        {
            this.watch = watch;
            this.socket = socket;
            this.endpoint = endpoint;

            this.outgoing = outgoing;
            this.incoming = new byte[outgoing.Length];
        }

        public void Start()
        {
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

                    socket.Send(outgoing, OnSent);
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

                    socket.Send(outgoing, OnSent);
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