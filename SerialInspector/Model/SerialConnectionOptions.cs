using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace SerialInspector.Model
{
    internal class SerialConnectionOptions
    {
        public string[] Ports
        {
            get;
            private set;
        }

        public IEnumerable<int> BaudRates
        {
            get
            {
                yield return 110;
                yield return 300;
                yield return 600;
                yield return 1200;
                yield return 2400;
                yield return 4800;
                yield return 9600;
                yield return 14400;
                yield return 19200;
                yield return 38400;
                yield return 57600;
                yield return 115200;
                yield return 128000;
                yield return 256000;
            }
        }

        public IEnumerable<Parity> Parities
        {
            get => (Parity[])Enum.GetValues(typeof(Parity));
        }

        public IEnumerable<int> DataBits
        {
            get
            {
                yield return 5;
                yield return 6;
                yield return 7;
                yield return 8;
            }
        }

        public IEnumerable<StopBits> StopBits
        {
            get => (StopBits[])Enum.GetValues(typeof(StopBits));
        }

        public SerialConnectionSettings Defaults
        {
            get
            {
                var settings = new SerialConnectionSettings();

                if (Ports?.Length > 0)
                {
                    settings.Port = Ports.First();
                }

                settings.BaudRate = 38400;
                settings.Parity = Parity.None;
                settings.DataBits = 8;
                settings.StopBits = System.IO.Ports.StopBits.One;

                return settings;
            }
        }

        // For unit testing
        internal SerialConnectionOptions(string[] ports)
        {
            Ports = ports;
        }

        internal SerialConnectionOptions(Func<bool> retryAction)
        {
            do
            {
                Ports = SerialPort.GetPortNames();
            } while (Ports.Length <= 0 && retryAction());
        }
    }
}