using System;

namespace SerialInspector
{
    internal class EntryPoint
    {
        [STAThread]
        public static int Main(string[] args)
        {
            using (var app = new App())
            {
                app.InitializeComponent();
                return app.Run();
            }
        }
    }
}
