# leak

```
leak download --torrent d:\debian-8.5.0-amd64-CD-1.iso.torrent
              --destination d:\leak

leak download --hash 883c6f02fc46188ac17ea49c13c3e9d97413a5a2
              --tracker http://bttracker.debian.org:6969/announce
              --destination d:\leak
````

````csharp
PeerClient client = new PeerClient(with =>
{
    with.Destination = "d:\\leak";
    with.Extensions.Metadata();
});

client.Start(with =>
{
    with.Hash = FileHash.Parse("883c6f02fc46188ac17ea49c13c3e9d97413a5a2");
    with.Trackers.Add("http://bttracker.debian.org:6969/announce");
});
````