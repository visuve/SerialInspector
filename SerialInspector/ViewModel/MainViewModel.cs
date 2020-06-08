using SerialInspector.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
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
            return MessageBox.Show(
                    "No serial devices found. Please check your connections.\n\nRetry?",
                    "Serial Inspector",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
        }

        public SerialConnectionOptions Options { get; } = new SerialConnectionOptions(Retry);

        public SerialConnectionSettings Settings { get; set; }

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
                MessageBox.Show($"Failed to open port: {e.Message}", "Serial Inspector");
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
                        Debug.WriteLine(Settings);

                        serialPort = new SerialPort(
                            Settings.Port,
                            Settings.BaudRate,
                            Settings.Parity,
                            Settings.DataBits,
                            Settings.StopBits);
                        serialPort.NewLine = "\r\n";

                        Messages.Clear();

                        keepRunning = true;
                        serialReaderThread = new Thread(ReadSerial);
                        serialReaderThread.Start();
                    }, x => !string.IsNullOrEmpty(Settings.Port) && !IsRunning);
                }

                return run;
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
            Settings = SettingsUtility.Get(Options);

            FirstChunkMath = "$A + $B + $C + $D / 256";
            SecondChunkMath = "$E + $F + $G + $H / 256";

            Messages = new ObservableSet<SerialMessage>(new SerialMessageSameIdentifier());
            MessageViewSource = CollectionViewSource.GetDefaultView(Messages);
            MessageViewSource.Filter = FilterMessages;
        }

        public void Dispose()
        {
            keepRunning = false;

            SettingsUtility.SaveIfNeeded(Settings);

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