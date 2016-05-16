using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
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

        public void Process(Action<EventProcessorConfigurator> callback)
        {
            EventProcessorConfigurator configurator = new EventProcessorConfigurator();
            EventProcessor processor = new EventProcessor(configurator);

            string[] files = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
            EventEnvelope[] source = files.Select(ToEnvelope).OrderBy(x => x.Name).ToArray();

            callback.Invoke(configurator);

            foreach (EventEnvelope envelope in source)
            {
                string data = File.ReadAllText(envelope.Path);
                dynamic payload = JsonConvert.DeserializeObject(data);

                processor.Process(payload.type.ToString(), payload);
            }
        }

        private static EventEnvelope ToEnvelope(string file)
        {
            return new EventEnvelope
            {
                Name = Path.GetFileName(file),
                Path = file
            };
        }

        private class EventEnvelope
        {
            public string Name { get; set; }

            public string Path { get; set; }
        }
    }
}