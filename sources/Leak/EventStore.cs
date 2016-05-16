using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Leak
{
    public class EventStore
    {
        private readonly string directory;

        public EventStore(string directory)
        {
            this.directory = directory;
        }

        public void Publish(string type, object data)
        {
            object payload = new
            {
                type = type,
                happened = DateTime.Now.ToUniversalTime(),
                data = data
            };

            string text = JsonConvert.SerializeObject(payload, Formatting.Indented);
            string name = $"{DateTime.Now.ToFileTimeUtc()}.json";
            string path = Path.Combine(directory, type, name);

            File.WriteAllText(path, text, Encoding.UTF8);
        }
    }
}