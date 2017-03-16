# leak

## command line
```
leak download --hash 73b38c5f82a28d47efef94c04d0a839b180f9ca0
              --trackers http://bttracker.debian.org:6969/announce
              --destination d:\leak

options:

    --connector (on|off) (default: on)

        Actively search for peers and connect to them.

    --listener (on|off) (default: off)

        Listen to incomming connections and accept them.

    --port #value (default: random)

        Listen on specified port.

    --accept #countries (default: nothing)

        Accepts only peers from the given countries; example: RU UA

    --metadata (on|off) (default: on)

        Use ut_metadata extension.

    --exchange (on|off) (default: on)

        Use ut_pex extension.

    --strategy (sequential|rarest-first) (default: rarest-first)

        Schedule pieces using algorithm.
````

## csharp code
````csharp
string tracker = "http://bttracker.debian.org:6969/announce";
FileHash hash = FileHash.Parse("883c6f02fc46188ac17ea49c13c3e9d97413a5a2");

using (SwarmClient client = new SwarmClient())
{
    SwarmNotification notification = null;
    SwarmSession session = await client.Connect(hash, tracker);

    session.Download("d:\\leak");

    do
    {
        notification = await session.Next();
    }
    while (notification.Type != SwarmNotificationType.DataCompleted)
}
````