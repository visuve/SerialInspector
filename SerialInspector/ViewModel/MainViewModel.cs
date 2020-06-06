using SerialInspector.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
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

        public ObservableSet<SerialMessage> messages;

        public ObservableSet<SerialMessage> Messages
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

        public ICollectionView MessageViewSource
        {
            get;
            private set;
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
                    SerialMessage message = SerialMessage.Parse(line, FirstChunkMath, SecondChunkMath);

                    var addItem = new Action(() => messages.Add(message));
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

            serialPort?.Close();
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

                        Messages.Clear();

                        keepRunning = true;
                        serialReaderThread = new Thread(ReadSerial);
                        serialReaderThread.Start();
                    }, x => !string.IsNullOrEmpty(SelectedPort) && !IsRunning);
                }

                return run;
            }
        }

        string identifierFilter;

        public string IdentifierFilter
        {
            get
            {
                return identifierFilter;
            }
            set
            {
                if (value != identifierFilter)
                {
                    identifierFilter = value;
                    MessageViewSource.Refresh();
                }
            }
        }

        private bool FilterMessages(object serialMessageObject)
        {
            if (string.IsNullOrEmpty(IdentifierFilter))
            {
                return true;
            }

            var serialMessage = serialMessageObject as SerialMessage;

            if (serialMessage == null)
            {
                throw new ArgumentException("Object is null or not a SerialMessage!");
            }

            return serialMessage.Identifier.StartsWith(IdentifierFilter);
        }

        internal MainViewModel()
        {
            AvailablePorts = SerialPort.GetPortNames();

            if (AvailablePorts?.Length > 0)
            {
                SelectedPort = AvailablePorts.First();
            }

            SelectedBaudRate = 38400;
            SelectedParity = Parity.None;
            SelectedDataBits = 8;
            SelectedStopBitCount = StopBits.One;

            FirstChunkMath = "$A + $B + $C + $D / 256";
            SecondChunkMath = "$E + $F + $G + $H / 256";

            Messages = new ObservableSet<SerialMessage>(new SerialMessageSameIdentifier());
            MessageViewSource = CollectionViewSource.GetDefaultView(Messages);
            MessageViewSource.Filter = FilterMessages;
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