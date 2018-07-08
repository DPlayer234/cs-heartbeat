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
    ///     An instantiable Engine object.
    /// </summary>
    public class EngineInstance : Game
    {
        /// <summary> The stack of contained <seealso cref="GameState"/>s. </summary>
        private Stack<GameState> gameStates = new Stack<GameState>();

        /// <summary> Checking for GameState changes </summary>
        private GameStateChange checkGameState;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EngineInstance"/> class.
        /// </summary>
        public EngineInstance(EngineConfig config) : base()
        {
            this.GraphicsDeviceManager = new GraphicsDeviceManager(this);

            this.Content.RootDirectory = config.ContentRoot;
        }

        /// <summary> Delegate Type for GameState changes </summary>
        private delegate void GameStateChange();

        /// <summary>
        ///     The active instance of the <see cref="EngineInstance"/>.
        ///     This is null if <seealso cref="Create"/> was not called.
        /// </summary>
        public static EngineInstance Instance { get; private set; }

        /// <summary> The assigned <seealso cref="Microsoft.Xna.Framework.GraphicsDeviceManager"/>. </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        /// <summary> The used <seealso cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/>. </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        ///     The active <seealso cref="GameState"/>.
        /// </summary>
        public GameState ActiveGameState
        {
            get
            {
                return this.gameStates.Count > 0 ? this.gameStates.Peek() : null;
            }
        }

        /// <summary>
        ///     Pushes a <seealso cref="GameState"/> to the execution stack.
        /// </summary>
        /// <typeparam name="T">The type of the game state</typeparam>
        /// <param name="gameState">The game state to push</param>
        /// <returns>The added game state</returns>
        public T PushGameState<T>(T gameState) where T : GameState
        {
            this.checkGameState += delegate
            {
                this.ActiveGameState?.OnPause();

                this.gameStates.Push(gameState);

                gameState.Engine = this;

                gameState.Initialize();
                gameState.OnResume();
            };

            return gameState;
        }

        /// <summary>
        ///     Pops the <seealso cref="ActiveGameState"/>.
        /// </summary>
        public void PopGameState()
        {
            GameState gameState = this.ActiveGameState;

            if (gameState != null)
            {
                this.gameStates.Pop();

                gameState.OnPause();
                gameState.TrulyDestroy();
            }

            this.ActiveGameState?.OnResume();
        }

        protected override void Initialize()
        {
            // Initialization

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);

            // Load via this.Content
        }

        protected override void UnloadContent()
        {
            // Non-content Manager
        }

        protected override void Update(GameTime gameTime)
        {
            this.checkGameState?.Invoke();
            this.checkGameState = null;

            this.ActiveGameState?.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.SpriteBatch.Begin();

            this.ActiveGameState?.Draw();

            this.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
