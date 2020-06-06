using System;

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
                Console.Error.WriteLine($"An exception occurred: {e.Message}");
                return e.HResult;
            }
        }
    }
}
