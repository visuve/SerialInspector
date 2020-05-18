using SerialInspector.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SerialInspector
{
    internal class MainViewModel : PropertyNotifyBase, IDisposable
    {
        private Thread serialReaderThread;
        private SerialPort serialPort;
        private bool keepRunning;

        public string[] AvailablePorts
        {
            get;
            private set;
        }

        public string SelectedPort
        {
            get; set;
        }

        public IEnumerable<int> BaudRates
        {
            get
            {
                yield return 300;
                yield return 600;
                yield return 1200;
                yield return 2400;
                yield return 4800;
                yield return 9600;
                yield return 14400;
                yield return 19200;
                yield return 28800;
                yield return 38400;
                yield return 57600;
                yield return 115200;
            }
        }

        public int SelectedBaudRate
        {
            get;
            set;
        }

        public IEnumerable<Parity> Parities
        {
            get => (Parity[])Enum.GetValues(typeof(Parity));
        }

        public Parity SelectedParity
        {
            get; set;
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

        public int SelectedDataBits
        {
            get;
            set;
        }

        public IEnumerable<StopBits> StopBitCount
        {
            get => (StopBits[])Enum.GetValues(typeof(StopBits));
        }

        public StopBits SelectedStopBitCount
        {
            get;
            set;
        }

        public bool IsRunning => serialPort?.IsOpen == true;

        public ObservableDictionary<string, DataChunk> messages;

        public ObservableDictionary<string, DataChunk> Messages
        {
            get
            {
                return messages;
            }
            private set
            {
                messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public string FirstChunkMath
        {
            get;
            set;
        }

        public string SecondChunkMath
        {
            get;
            set;
        }

        private void ReadSerial()
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to open port: {e.Message}", "SerialInspector");
                return;
            }

            OnPropertyChanged(nameof(IsRunning));
            int errorCount = 0;

            while (keepRunning && serialPort.IsOpen && errorCount < 50)
            {
                try
                {
                    string line = serialPort.ReadLine();
                    string identifier = line.Substring(0, 8);
                    string chunks = line.Substring(9, 17);
                    var chunk = new DataChunk(chunks, FirstChunkMath, SecondChunkMath);

                    var addItem = new Action(() => Messages[identifier] = chunk);
                    Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, addItem);
                }
                catch (IOException e)
                {
                    MessageBox.Show($"I/O exception occurred: {e.Message}", "SerialInspector");
                    break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"An exception occurred: {e.Message}");
                    ++errorCount;
                    continue;
                }
            }

            serialPort.Close();
            OnPropertyChanged(nameof(IsRunning));
        }

        private ICommand run;

        public ICommand Run
        {
            get
            {
                if (run == null)
                {
                    run = new RelayCommand(x =>
                    {
                        Debug.WriteLine(SelectedPort);
                        Debug.WriteLine(SelectedBaudRate);
                        Debug.WriteLine(SelectedParity);
                        Debug.WriteLine(SelectedDataBits);
                        Debug.WriteLine(SelectedStopBitCount);

                        serialPort = new SerialPort(
                            SelectedPort,
                            SelectedBaudRate,
                            SelectedParity,
                            SelectedDataBits,
                            SelectedStopBitCount);
                        serialPort.NewLine = "\r\n";

                        Messages = new ObservableDictionary<string, DataChunk>();

                        keepRunning = true;
                        serialReaderThread = new Thread(ReadSerial);
                        serialReaderThread.Start();
                    }, x => !string.IsNullOrEmpty(SelectedPort) && !IsRunning);
                }

                return run;
            }
        }

        internal MainViewModel()
        {
            AvailablePorts = SerialPort.GetPortNames();

            if (AvailablePorts?.Length > 0)
            {
                SelectedPort = AvailablePorts.First();
            }

            SelectedBaudRate = 9600;
            SelectedParity = Parity.None;
            SelectedDataBits = 8;
            SelectedStopBitCount = StopBits.One;

            FirstChunkMath = "%FIRST_CHUNK% / 256";
            SecondChunkMath = "%SECOND_CHUNK% / 256";
        }

        public void Dispose()
        {
            keepRunning = false;

            if (serialReaderThread != null)
            {
                serialReaderThread.Join(1000); // TODO: this is hacky
            }

            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
        }
    }
}