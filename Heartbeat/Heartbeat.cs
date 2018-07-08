using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    /// <summary>
    ///     Static access to the Engine.
    /// </summary>
    public static class Engine
    {
        /// <summary> The used instance of the engine </summary>
        private static EngineInstance instance;

        /// <summary> The active <seealso cref="GameState"/>. </summary>
        public static GameState ActiveGameState
        {
            get
            {
                return Engine.instance.ActiveGameState;
            }
        }

        /// <summary>
        ///     Initializes the Engine.
        /// </summary>
        public static void Initialize()
        {
            Engine.Initialize(EngineConfig.Default);
        }

        /// <summary>
        ///     Initializes the Engine.
        /// </summary>
        /// <param name="config">The used configuration</param>
        public static void Initialize(EngineConfig config)
        {
            if (Engine.instance != null) throw new Exception();

            Engine.instance = new EngineInstance(config);
        }

        /// <summary>
        ///     Starts the engine. This will block until the game has finished.
        /// </summary>
        public static void Start()
        {
            using (Engine.instance)
            {
                Engine.instance.Run();
            }
        }

        /// <summary>
        ///     Pushes a <seealso cref="GameState"/> to the execution stack.
        /// </summary>
        /// <typeparam name="T">The type of the game state</typeparam>
        /// <param name="gameState">The game state to push</param>
        /// <returns>The added game state</returns>
        public static T PushGameState<T>(T gameState) where T : GameState
        {
            return Engine.instance.PushGameState(gameState);
        }

        /// <summary>
        ///     Pops the <seealso cref="ActiveGameState"/>.
        /// </summary>
        public static void PopGameState()
        {
            Engine.instance.PopGameState();
        }
    }
}
