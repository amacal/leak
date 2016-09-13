# leak

## command line
```
leak download --torrent d:\debian-8.5.0-amd64-CD-1.iso.torrent
              --destination d:\leak

leak download --hash 883c6f02fc46188ac17ea49c13c3e9d97413a5a2
              --tracker http://bttracker.debian.org:6969/announce
              --destination d:\leak

options:

    --connector (on|off) (default: on)

        Actively search for peers and connect to them.

    --listener (on|off) (default: off)

        Listen to incomming connections and accept them.

    --port #value (default: 8080)

        Listen on specified port.

    --metadata (on|off) (default: on)

        Use ut_metadata extension.

    --peer-exchange (on|off) (default: on)

        Use ut_pex extension.

    --download (sequential|rarest-first) (default: rarest-first)

        Schedule pieces using algorithm.

    --logging (off|normal|verbose) (default: normal)

        Show only logs with requested severity.
````

## csharp code
````csharp
PeerClient client = new PeerClient(with =>
{
    with.Destination = "d:\\leak";
    with.Metadata.Enable();
    with.PeerExchange.Enable();
});

client.Start(with =>
{
    with.Hash = FileHash.Parse("883c6f02fc46188ac17ea49c13c3e9d97413a5a2");
    with.Trackers.Add("http://bttracker.debian.org:6969/announce");
});
````