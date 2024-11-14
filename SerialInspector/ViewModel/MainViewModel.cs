using SerialInspector.Model;
using System;
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

        private static bool Retry()
        {
#if DEBUG
            return false;
#else
            return MessageBox.Show(
                    "No serial devices found. Please check your connections.\n\nRetry?",
                    "Serial Inspector",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
#endif
        }

        public SerialConnectionOptions Options { get; } = new SerialConnectionOptions(Retry);

        public SerialConnectionSettings Settings { get; set; }

        public bool IsConnected => serialPort?.IsOpen == true;

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
                MessageBox.Show($"Failed to open port: {e.Message}", "Serial Inspector");
                return;
            }

            OnPropertyChanged(nameof(IsConnected));
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
                    MessageBox.Show($"I/O exception occurred: {e.Message}", "Serial Inspector");
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
            OnPropertyChanged(nameof(IsConnected));
        }

        public SerialMessage UserMessage { get; set; } = SerialMessage.Parse("DEADBEEF|FF-00-11-AA-BB-CC-DD-EE");

        private ICommand connect;

        public ICommand Connect
        {
            get
            {
                if (connect == null)
                {
                    connect = new RelayCommand(x =>
                    {
                        Debug.WriteLine(Settings);

                        serialPort = new SerialPort(
                            Settings.Port,
                            Settings.BaudRate,
                            Settings.Parity,
                            Settings.DataBits,
                            Settings.StopBits);

                        serialPort.Handshake = Settings.FlowControl;
                        serialPort.NewLine = "\r\n";

                        Messages.Clear();

                        keepRunning = true;
                        serialReaderThread = new Thread(ReadSerial);
                        serialReaderThread.Start();
                    }, x => !string.IsNullOrEmpty(Settings.Port) && !IsConnected);
                }

                return connect;
            }
        }

        private ICommand send;

        public ICommand Send
        {
            get
            {
                if (send == null)
                {
                    send = new RelayCommand(x =>
                    {
                        Debug.WriteLine(UserMessage.ToString());
                        serialPort.Write(UserMessage.ToString());
                    }, x => IsConnected);
                }

                return send;
            }
        }

        private string identifierFilter;

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
            Settings = SettingsUtility.Get();

            if (string.IsNullOrEmpty(Settings.Port))
            {
                Settings.Port = Options.Ports.FirstOrDefault();
            }

            FirstChunkMath = "$A + $B + $C + $D / 256";
            SecondChunkMath = "$E + $F + $G + $H / 256";

            Messages = new ObservableSet<SerialMessage>(new SerialMessageSameIdentifier());
            MessageViewSource = CollectionViewSource.GetDefaultView(Messages);
            MessageViewSource.Filter = FilterMessages;
        }

        public void Dispose()
        {
            keepRunning = false;

            SettingsUtility.SaveIfValid(Settings);

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