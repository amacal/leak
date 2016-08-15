using Leak.Core.Bencoding;
using Leak.Core.Common;
using Leak.Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leak.Core.Cando.PeerExchange
{
    public class PeerExchangeHandler : CandoHandler
    {
        private readonly PeerExchangeConfiguration configuration;

        public PeerExchangeHandler(Action<PeerExchangeConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerExchangeCallbackNothing();
            });
        }

        public bool CanHandle(string name)
        {
            return name == "ut_pex";
        }

        public void OnHandshake(PeerHash peer, BencodedValue handshake)
        {
        }

        public void OnMessage(PeerHash peer, Extended payload)
        {
            BencodedValue value = Bencoder.Decode(payload.Data);
            byte[] added = value.Find("added", x => x?.Data?.GetBytes());
            List<PeerAddress> peers = new List<PeerAddress>();

            if (added != null)
            {
                for (int i = 0; i < added.Length; i += 6)
                {
                    string host = GetHost(added, i);
                    int port = GetPort(added, i);

                    peers.Add(new PeerAddress(host, port));
                }
            }

            configuration.Callback.OnMessage(peer, new PeerExchangeMessage(peers.ToArray()));
        }

        private static string GetHost(byte[] data, int offset)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(data[0 + offset]);
            builder.Append('.');
            builder.Append(data[1 + offset]);
            builder.Append('.');
            builder.Append(data[2 + offset]);
            builder.Append('.');
            builder.Append(data[3 + offset]);

            return builder.ToString();
        }

        private static int GetPort(byte[] data, int offset)
        {
            return data[4 + offset] * 256 + data[5 + offset];
        }
    }
}