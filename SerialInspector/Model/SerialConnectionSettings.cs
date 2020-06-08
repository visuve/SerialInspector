using System.IO.Ports;
using System.Text.Json.Serialization;

namespace SerialInspector.Model
{
    internal class SerialConnectionSettings
    {
        [JsonPropertyName("port")]
        public string Port { get; set; }

        [JsonPropertyName("baud_rate")]
        public int BaudRate { get; set; }

        [JsonPropertyName("parity")]
        public Parity Parity { get; set; }

        [JsonPropertyName("data_bits")]
        public int DataBits { get; set; }

        [JsonPropertyName("stop_bits")]
        public StopBits StopBits { get; set; }
    }
}