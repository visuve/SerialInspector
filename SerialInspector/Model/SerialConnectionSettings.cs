using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Ports;
using System.Text.Json.Serialization;

namespace SerialInspector.Model
{
    internal class SerialConnectionSettings
    {
        [JsonPropertyName("port"), PortName()]
        public string Port { get; set; }

        [JsonPropertyName("baud_rate"), List(110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000)]
        public int BaudRate { get; set; } = 115200;

        [JsonPropertyName("parity"), Enumeration(typeof(Parity))]
        public Parity Parity { get; set; } = Parity.None;

        [JsonPropertyName("data_bits"), Range(5, 8)]
        public int DataBits { get; set; } = 8;

        [JsonPropertyName("stop_bits"), Enumeration(typeof(StopBits))]
        public StopBits StopBits { get; set; } = StopBits.One;

        [JsonPropertyName("flow_control"), Enumeration(typeof(Handshake))]
        public Handshake FlowControl { get; set; } = Handshake.None;

        internal void Validate()
        {
            var context = new ValidationContext(this);
            Validator.ValidateObject(this, context, true);
        }

        internal bool TryValidate()
        {
            try
            {
                Validate();
                return true;
            }
            catch (ValidationException e)
            {
                Debug.WriteLine($"Validation failed: {e.Message}");
            }

            return false;
        }
    }
}