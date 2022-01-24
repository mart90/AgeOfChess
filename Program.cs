using System;
using System.IO;

namespace AgeOfChess
{
    // dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using (var game = new UIController())
                    game.Run();
            }
            catch (Exception e)
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory.ToString() + "/ErrorLog.txt", e.ToString());
            }
        }
    }
}
