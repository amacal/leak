using FluentAssertions;
using Leak.Core.Net;
using NUnit.Framework;
using System;
using System.Threading;

namespace Leak.Core.Tests.Net
{
    [TestFixture]
    public class EncryptedPeerNegotiatorTests
    {
        [Test]
        public void CanTransferSomeDataAfterNegotiation()
        {
            byte[] hash = Bytes.Random(20);
            Network network = new Network();

            Attendant initiator = new Attendant(hash, network.Input, network.Output, "abc");
            Attendant receiver = new Attendant(hash, network.Output, network.Input, "cde");

            receiver.Negotiator.Passive(receiver.Channel);
            initiator.Negotiator.Active(initiator.Channel);

            network.Flush();

            initiator.Received.Should().Be("cde");
            receiver.Received.Should().Be("abc");
        }

        private class Attendant
        {
            private readonly Channel channel;
            private readonly PeerHandshake handshake;
            private readonly PeerNegotiator negotiator;
            private string received;

            public Attendant(byte[] hash, NetworkStream input, NetworkStream output, string data)
            {
                handshake = new PeerHandshake(hash, Bytes.Random(20));
                negotiator = new EncryptedPeerNegotiator(handshake);
                channel = new Channel(input, output, System.Text.Encoding.ASCII.GetBytes(data), x => received = System.Text.Encoding.ASCII.GetString(x));
            }

            public Channel Channel
            {
                get { return channel; }
            }

            public PeerHandshake Handshake
            {
                get { return handshake; }
            }

            public PeerNegotiator Negotiator
            {
                get { return negotiator; }
            }

            public string Received
            {
                get { return received; }
            }
        }

        private class Network
        {
            private readonly NetworkStream input;
            private readonly NetworkStream output;
            private readonly Timer timer;

            public Network()
            {
                this.input = new NetworkStream();
                this.output = new NetworkStream();
                this.timer = new Timer(Tick);
                this.timer.Change(0, 3);
            }

            public NetworkStream Input
            {
                get { return input; }
            }

            public NetworkStream Output
            {
                get { return output; }
            }

            private void Tick(object state)
            {
                timer.Change(-1, 3);

                input.Trigger();
                output.Trigger();

                timer.Change(0, 3);
            }

            public void Flush()
            {
                int count = 2000;

                while (count > 0 && (input.IsEmpty() == false || output.IsEmpty() == false))
                {
                    count--;
                    Thread.Sleep(1);
                }
            }
        }

        private class NetworkStream
        {
            private int size;
            private byte[] data;
            private Func<byte[], bool> predicate;
            private Action<byte[]> callback;

            public NetworkStream()
            {
                this.data = new byte[0];
            }

            public void Push(byte[] bytes)
            {
                lock (this)
                {
                    Array.Resize(ref data, data.Length + bytes.Length);
                    Array.Copy(bytes, 0, data, data.Length - bytes.Length, bytes.Length);
                }
            }

            public void Receive(Func<byte[], bool> predicate, Action<byte[]> callback)
            {
                lock (this)
                {
                    this.predicate = predicate;
                    this.callback = callback;
                }
            }

            public void Remove(int count)
            {
                lock (this)
                {
                    byte[] copy = new byte[data.Length - count];
                    Array.Copy(data, count, copy, 0, copy.Length);

                    data = copy;
                    size = size - count;
                }
            }

            public void Trigger()
            {
                lock (this)
                {
                    if (data.Length > 0 && callback != null)
                    {
                        while (size < data.Length)
                        {
                            byte[] payload = data.ToBytes(0, ++size);

                            if (predicate.Invoke(payload))
                            {
                                Action<byte[]> current = callback;

                                size--;
                                callback = null;

                                current.Invoke(payload);
                                break;
                            }
                        }
                    }
                }
            }

            public bool IsEmpty()
            {
                lock (this)
                {
                    return data.Length == 0;
                }
            }
        }

        private class Channel : PeerNegotiatorAware
        {
            private readonly NetworkStream input;
            private readonly NetworkStream output;
            private readonly Action<byte[]> onContinue;
            private readonly byte[] data;

            public Channel(NetworkStream input, NetworkStream output, byte[] data, Action<byte[]> onContinue)
            {
                this.input = input;
                this.output = output;
                this.onContinue = onContinue;
                this.data = data;
            }

            public void Continue(Func<PeerMessage, PeerMessage> encrypt, Func<PeerMessage, PeerMessage> decrypt, Action<PeerBuffer, int> remove)
            {
                output.Push(encrypt.Invoke(new PeerMessage(data)).ToBytes());

                input.Receive(
                    x => x.Length >= 3,
                    x => onContinue.Invoke(decrypt.Invoke(new PeerMessage(x)).ToBytes()));
            }

            public void Handle(PeerHandshake handshake)
            {
            }

            public void Receive(Func<PeerMessage, bool> predicate, Action<PeerMessage> callback)
            {
                Func<byte[], bool> handler = bytes =>
                {
                    return predicate.Invoke(new PeerMessage(bytes));
                };

                input.Receive(handler, bytes =>
                {
                    callback.Invoke(new PeerMessage(bytes));
                });
            }

            public void Remove(int length)
            {
                input.Remove(length);
            }

            public void Send(PeerMessageFactory data)
            {
                output.Push(data.GetMessage().ToBytes());
            }

            public void Terminate()
            {
                throw new NotImplementedException();
            }
        }
    }
}