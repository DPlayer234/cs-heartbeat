using System;
using Heartbeat;

namespace TestGame
{
#if WINDOWS || LINUX
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
            Engine.Initialize();

            Engine.PushGameState(new GameState());

            Engine.Start();
        }
    }
#endif
}
