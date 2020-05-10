using System;
using System.Windows;

namespace SerialInspector
{
    public partial class App : Application, IDisposable
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Activate();
        }

        public void Dispose()
        {
            (MainWindow as IDisposable)?.Dispose();
            MainWindow = null;
        }
    }
}