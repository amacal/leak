using System;
using Leak.Client.Tracker;

namespace Leak.Announce
{
    public class Logger : TrackerLogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
        }
    }
}