using System;

namespace AgeOfChess
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new UIController())
                game.Run();
        }
    }
}
