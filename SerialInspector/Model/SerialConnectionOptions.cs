using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace SerialInspector.Model
{
    internal class SerialConnectionOptions
    {
        public string[] Ports
        {
            get;
            private set;
        }

        public static IEnumerable<int> BaudRates => new int[]
        {
            110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000
        };

        public static IEnumerable<Parity> Parities => (IEnumerable<Parity>)Enum.GetValues(typeof(Parity));

        public static IEnumerable<int> DataBits => new int[]
        {
            5, 6, 7, 8
        };

        public static IEnumerable<StopBits> StopBits => (IEnumerable<StopBits>)Enum.GetValues(typeof(StopBits));

        public static IEnumerable<Handshake> FlowControl => (IEnumerable<Handshake>)Enum.GetValues(typeof(Handshake));

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