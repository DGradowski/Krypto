//Jakub Gawrysiak - 252935
//Dawid Gradowski - 251524

using MHCipherUI;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MHCipherForm());
    }
}
