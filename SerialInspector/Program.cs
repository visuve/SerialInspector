using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;

internal class Program
{
    private class Inspector : IDisposable
    {
        private struct DataChunk
        {
            internal DataChunk(string raw)
            {
                string[] chunks = raw.Split('-');
                First = uint.Parse(chunks[0], NumberStyles.HexNumber);
                Second = uint.Parse(chunks[1], NumberStyles.HexNumber);
            }

            public override string ToString()
            {
                return $"{First}-{Second}";
            }

            private uint First;
            private uint Second;
        }

        private int errorCount = 0;
        private SerialPort serialPort;
        private Dictionary<string, DataChunk> messages;

        public void Dispose()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
        }

        internal Inspector()
        {
            serialPort = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
            serialPort.NewLine = "\r\n";
            messages = new Dictionary<string, DataChunk>();
        }

        internal void Run()
        {
            serialPort.Open();

            while (serialPort.IsOpen && errorCount < 50)
            {
                try
                {
                    string line = serialPort.ReadLine();
                    string identifier = line.Substring(0, 8);
                    string chunks = line.Substring(9, 17);
                    messages[identifier] = new DataChunk(chunks);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"An exception occurred: {e.Message}");
                    ++errorCount;
                    continue;
                }

                Console.Clear();

                foreach (var message in messages)
                {
                    Console.WriteLine($"{message.Key}\t{message.Value}");
                }
            }
        }

        private string ToHexString(string data)
        {
            return string.Join("", data.Select(c => string.Format("{0:X2}", Convert.ToInt32(c))));
        }
    }

    [STAThread]
    public static void Main(string[] args)
    {
        using (var inspector = new Inspector())
        {
            inspector.Run();
        }
    }
}