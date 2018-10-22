using FredflixAndChell.Shared;
using System;

namespace FredflixAndChell.CrossPlatform
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new BroGame())
            {
                game.Run();
            }
        }
    }
}
