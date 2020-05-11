using System;
using System.Windows;
using System.Windows.Controls;

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

        private void ApplyClicked(object sender, RoutedEventArgs e)
        {
            if (firstChunkMathTextBox.Text.Contains("%FIRST_CHUNK%"))
            {
                firstChunkMathTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
            else
            {
                MessageBox.Show("First chunk math is missing %FIRST_CHUNK% placeholder!", "Serial Inspector");
            }

            if (secondChunkMathTextBox.Text.Contains("%SECOND_CHUNK%"))
            {
                secondChunkMathTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
            else
            {
                MessageBox.Show("Second chunk math is missing %SECOND_CHUNK% placeholder!", "Serial Inspector");
            }
        }
    }
}