using System;
using System.Windows;

namespace SerialInspector
{
    public partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        public void Dispose()
        {
            (DataContext as IDisposable)?.Dispose();
            DataContext = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            (DataContext as IDisposable)?.Dispose();
            DataContext = null;
        }
    }
}