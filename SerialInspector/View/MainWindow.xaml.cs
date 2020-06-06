using System;
using System.Linq;
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

        private void ApplyMathClicked(object sender, RoutedEventArgs e)
        {
            string[] firstChunkVariables = { "$A", "$B", "$C", "$D" };

            if (firstChunkVariables.Any(c => firstChunkMathTextBox.Text.Contains(c)))
            {
                firstChunkMathTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
            else
            {
                MessageBox.Show("First chunk math has no placeholders!", "Serial Inspector");
            }

            string[] secondChunkVariables = { "$E", "$F", "$G", "$H" };

            if (secondChunkVariables.Any(c => secondChunkMathTextBox.Text.Contains(c)))
            {
                secondChunkMathTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
            else
            {
                MessageBox.Show("Second chunk math has no placeholders!", "Serial Inspector");
            }
        }

        private void ApplyFilterClicked(object sender, RoutedEventArgs e)
        {
            if (filterTextBox.Text.Length <= 8)
            {
                filterTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
            else
            {
                MessageBox.Show("Filter too long!", "Serial Inspector");
            }
        }
    }
}