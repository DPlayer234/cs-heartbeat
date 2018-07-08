using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Heartbeat
{
    /// <summary>
    ///     The core of the Engine, the <seealso cref="Heartbeat"/>, so to speak.
    ///     It is mostly accessed via static properties and methods, but
    ///     <seealso cref="Engine.Game"/> and <seealso cref="Engine.Instance"/>
    ///     are available for more direct access.
    /// </summary>
    public sealed class Engine : Game
    {
        /// <summary> The queued game state transitions </summary>
        private static GameStateChange gameStateTransitions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Engine"/> class.
        /// </summary>
        private Engine(EngineConfig config) : base()
        {
            Engine.GraphicsDeviceManager = new GraphicsDeviceManager(this);

            this.Content.RootDirectory = config.ContentRoot;
        }

        /// <summary> Delegate Type for GameState changes </summary>
        private delegate void GameStateChange();

        /// <summary>
        ///     The statuses the engine may have.
        /// </summary>
        public enum EngineStatus
        {
            /// <summary> The engine is inactive and was not created </summary>
            Inactive,

            /// <summary> The engine is idling </summary>
            Idle,

            /// <summary> The engine is updating </summary>
            Update,

            /// <summary> The engine is drawing </summary>
            Draw,
            
            /// <summary> The engine is handling GameStates </summary>
            HandlingGameStates
        }

        /// <summary>
        ///     The active instance of the <see cref="Engine"/>.
        ///     This is null if the Engine was not started.
        /// </summary>
        public static Engine Instance { get; private set; }

        /// <summary> The current status of the Engine </summary>
        public static EngineStatus Status { get; private set; } = EngineStatus.Inactive;

        /// <summary> The assigned <seealso cref="Microsoft.Xna.Framework.GraphicsDeviceManager"/>. </summary>
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        /// <summary> The used <seealso cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/>. </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        ///     The active <seealso cref="GameState"/>.
        /// </summary>
        public static GameState ActiveGameState { get; private set; }

        /// <summary> The unscaled time since the last frame </summary>
        public static float UnscaledDeltaTime { get; private set; }

        /// <summary> The scaled time since the last frame </summary>
        public static float DeltaTime { get; private set; }

        /// <summary> The <seealso cref="GameState.TimeScale"/> of <seealso cref="ActiveGameState"/> </summary>
        public static float TimeScale
        {
            get
            {
                return Engine.ActiveGameState?.TimeScale ?? 1.0f;
            }
        }

        /// <summary>
        ///     The active <see cref="Instance"/> as a <see cref="Microsoft.Xna.Framework.Game"/>.
        /// </summary>
        public static Game Game
        {
            get
            {
                return Engine.Instance as Game;
            }
        }

        /// <summary>
        ///     Creates and initializes the Engine.
        /// </summary>
        /// <exception cref="InvalidOperationException"><seealso cref="Create"/> was called before</exception>
        public static void Create()
        {
            Engine.Create(EngineConfig.Default);
        }

        /// <summary>
        ///     Creates and initializes the Engine.
        /// </summary>
        /// <param name="config">The used configuration</param>
        /// <exception cref="InvalidOperationException"><seealso cref="Create(EngineConfig)"/> was called before</exception>
        public static void Create(EngineConfig config)
        {
            if (Engine.Instance != null) throw new InvalidOperationException(nameof(Create) + " was called before.");

            Engine.Instance = new Engine(config);

            Engine.Status = EngineStatus.Idle;
        }

        /// <summary>
        ///     Runs the Engine. You must have called <see cref="PushGameState{T}(T)"/> at this point already.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is no <seealso cref="ActiveGameState"/></exception>
        public new static void Run()
        {
            Engine.DoGameStateTransitions();

            if (Engine.ActiveGameState == null) throw new InvalidOperationException("There was no GameState pushed.");

            using (Engine.Game)
            {
                Engine.Game.Run();
            }
        }

        /// <summary>
        ///     Quick starts the Engine.
        ///     This includes creation, pushing a game state and running it.
        /// </summary>
        /// <typeparam name="T">The type of the GameState to push.</typeparam>
        public static void QuickStart<T>() where T : GameState, new()
        {
            Engine.Create();

            Engine.PushGameState(new T());

            Engine.Run();
        }

        /// <summary>
        ///     Quick starts the Engine.
        ///     This includes creation, pushing a game state and running it.
        /// </summary>
        /// <typeparam name="T">The type of the GameState to push.</typeparam>
        /// <param name="gameState">The GameState</param>
        public static void QuickStart<T>(T gameState) where T : GameState
        {
            Engine.Create();

            Engine.PushGameState(gameState);

            Engine.Run();
        }

        /// <summary>
        ///     Pushes a <seealso cref="GameState"/> to the execution stack.
        /// </summary>
        /// <typeparam name="T">The type of the game state</typeparam>
        /// <param name="gameState">The game state to push</param>
        /// <returns>The added game state</returns>
        /// <exception cref="ArgumentNullException"><paramref name="gameState"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="gameState"/> was used/pushed before already</exception>
        public static T PushGameState<T>(T gameState) where T : GameState
        {
            if (gameState == null) throw new ArgumentNullException(nameof(gameState));
            if (gameState.IsInUse) throw new InvalidOperationException("The given GameState was pushed/used already.");

            gameState.IsInUse = true;

            Engine.gameStateTransitions += delegate
            {
                // Pause last state
                if (Engine.ActiveGameState != null)
                {
                    Engine.ActiveGameState.NextState = gameState;

                    Engine.ActiveGameState.OnPause();

                    gameState.LastState = Engine.ActiveGameState;
                }

                // Set and initialize next state
                Engine.ActiveGameState = gameState;

                gameState.OnResume();
                gameState.Initialize();
            };

            return gameState;
        }

        /// <summary>
        ///     Pops the <seealso cref="ActiveGameState"/>.
        /// </summary>
        public static void PopGameState()
        {
            Engine.gameStateTransitions += delegate
            {
                // Store currently active state
                GameState lastActiveState = Engine.ActiveGameState;

                // If null, do nothing
                if (lastActiveState == null) return;

                // Pause the state
                lastActiveState.OnPause();

                // Change the active game state and initialize
                Engine.ActiveGameState = lastActiveState.LastState;

                if (Engine.ActiveGameState != null)
                {
                    Engine.ActiveGameState.NextState = null;
                    
                    Engine.ActiveGameState.OnResume();
                }

                // Destroy the last state
                lastActiveState.TrulyDestroy();
            };
        }

        protected override void Initialize()
        {
            // Initialization

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Engine.SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load via this.Content
        }

        protected override void UnloadContent()
        {
            // Non-content Manager
        }

        protected override void Update(GameTime gameTime)
        {
            Engine.SetGameTime(gameTime);

            Engine.DoGameStateTransitions();

            Engine.Status = EngineStatus.Update;

            Engine.ActiveGameState?.Update();

            base.Update(gameTime);

            Engine.Status = EngineStatus.Idle;
        }

        protected override void Draw(GameTime gameTime)
        {
            Engine.SetGameTime(gameTime);

            Engine.Status = EngineStatus.Draw;

            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            Engine.SpriteBatch.Begin();

            Engine.ActiveGameState?.Draw();

            Engine.SpriteBatch.End();

            base.Draw(gameTime);

            Engine.Status = EngineStatus.Idle;
        }

        /// <summary>
        ///     Executes all queued game state transitions.
        /// </summary>
        private static void DoGameStateTransitions()
        {
            Engine.Status = EngineStatus.HandlingGameStates;

            Engine.gameStateTransitions?.Invoke();
            Engine.gameStateTransitions = null;

            Engine.Status = EngineStatus.Idle;
        }

        /// <summary>
        ///     Sets <see cref="DeltaTime"/> and <see cref="UnscaledDeltaTime"/>.
        /// </summary>
        /// <param name="gameTime">The game time</param>
        private static void SetGameTime(GameTime gameTime)
        {
            double seconds = gameTime.ElapsedGameTime.TotalSeconds;

            Engine.UnscaledDeltaTime = (float)seconds;
            Engine.DeltaTime = (float)(Engine.TimeScale * seconds);
        }
    }
}
