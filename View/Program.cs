using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESXUI;

namespace View
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            // Włączenie stylów wizualnych i ustawienie renderowania tekstu
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Uruchomienie głównego okna aplikacji
            Application.Run(new DESXForm());
        }
    }
}
