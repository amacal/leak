# leak #

Leak is a torrent library for .NET 4.5 written only in C#. It implements its own IO layer to fully benefit from use of windows completion ports. It also delivers sample [end user tools](https://github.com/amacal/leak/wiki/End-user) to demonstrate all its features.

## downloads ##

The latest release of the leak library is [available on NuGet](https://www.nuget.org/packages/Leak.Core/) or can be [downloaded from GitHub](https://github.com/amacal/leak/releases).

## documentation ##

Documentation is hosted on GitHub at [https://github.com/amacal/leak/wiki](https://github.com/amacal/leak/wiki).

## license ##

Leak is Open Source software and is released under the [MIT license](https://github.com/amacal/leak/wiki/License). The license allows the use of Leak in free and commercial applications and libraries without restrictions.

## sample ##
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
