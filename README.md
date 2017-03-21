# leak #

Leak is a torrent library for .NET 4.5 written only in C#. It implements its own IO layer to fully benefit from use of windows completion ports. It also delivers sample end-user tools to demonstrate all its features.

## downloads ##

The latest release of the leak library is [available on NuGet](https://www.nuget.org/packages/Leak.Core/) or can be [downloaded from GitHub](https://github.com/amacal/leak/releases).

## documentation ##

Documentation is hosted on GitHub at [https://github.com/amacal/leak/wiki](https://github.com/amacal/leak/wiki).

## License ##

Leak is Open Source software and is released under the [MIT license](https://github.com/amacal/leak/wiki/License). The license allows the use of Leak in free and commercial applications and libraries without restrictions.

## end-user tools ##
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

    --accept #countries (default: all)

        Accept only peers from the given countries; example: RU UA

    --metadata (on|off) (default: on)

        Use ut_metadata extension.

    --exchange (on|off) (default: on)

        Use ut_pex extension.

    --strategy (sequential|rarest-first) (default: rarest-first)

        Schedule pieces using specified algorithm.

    --logging (compact|verbose) (default: compact)

        Control the number of logs printed out.
````

## sample C# code ##
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
