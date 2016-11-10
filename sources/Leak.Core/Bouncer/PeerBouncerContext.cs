using Geon;
using Geon.Formats;
using Geon.Readers;
using Geon.Sources;
using System;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerContext
    {
        private readonly object synchronized;
        private readonly PeerBouncerConfiguration configuration;
        private readonly PeerBouncerCollection collection;
        private readonly PeerBouncerGeolocator geolocator;

        public PeerBouncerContext(Action<PeerBouncerConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerBouncerCallbackNothing();
                with.Countries = new string[0];
                with.Connections = 128;
            });

            synchronized = new object();
            collection = new PeerBouncerCollection();

            if (configuration.Countries.Length > 0)
            {
                Geo geo = GeoFactory.Open(with =>
                {
                    with.Format(new ZipFormat());
                    with.Source(new MaxMindSource());
                    with.Reader(new CsvReader());
                });

                geolocator = new PeerBouncerGeolocator(geo, configuration.Countries);
            }
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public PeerBouncerConfiguration Configuration
        {
            get { return configuration; }
        }

        public PeerBouncerCallback Callback
        {
            get { return configuration.Callback; }
        }

        public PeerBouncerCollection Collection
        {
            get { return collection; }
        }

        public PeerBouncerGeolocator Geolocator
        {
            get { return geolocator; }
        }
    }
}