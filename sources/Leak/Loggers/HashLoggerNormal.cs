using System;

namespace Leak.Loggers
{
    public class HashLoggerNormal : HashLogger
    {
        protected override void Handle(string name, dynamic payload, Action next)
        {
            switch (name)
            {
                case "file-scheduled":
                    Console.WriteLine($"{payload.Hash}: scheduled");
                    break;

                case "file-announced":
                    Console.WriteLine($"{payload.Hash}: announced; peers={payload.Count}");
                    break;

                case "metadata-measured":
                    Console.WriteLine($"{payload.Hash}: metadata measured; size={payload.Size}");
                    break;

                case "file-discovered":
                    Console.WriteLine($"{payload.Hash}: discovered");
                    break;

                case "file-started":
                    Console.WriteLine($"{payload.Hash}: started");
                    break;

                case "file-initialized":
                    Console.WriteLine($"{payload.Hash}: initialized; completed={payload.Completed}; total={payload.Total}");
                    break;

                case "file-changed":
                    Console.WriteLine($"{payload.Hash}: changed; completed={payload.Completed}; total={payload.Total}");
                    break;

                case "file-completed":
                    Console.WriteLine($"{payload.Hash}: completed");
                    break;
            }

            next.Invoke();
        }
    }
}