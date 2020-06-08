using System;
using System.Windows;

namespace SerialInspector
{
    internal class EntryPoint
    {
        [STAThread]
        public static int Main(string[] args)
        {
            try
            {
                using (var app = new App())
                {
                    app.InitializeComponent();
                    return app.Run();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "An exception occurred which Serial Inspector was unable to recover.\n\n" +
                    $"Exception details: {e.Message}.\n\n" +
                    "Please restart the application in order to continue.",
                    "Serial Inspector",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);

                return e.HResult;
            }
        }
    }
}