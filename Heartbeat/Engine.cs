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
    ///     The Engine itself.
    /// </summary>
    public class Engine : Game
    {
        /// <summary>
        ///     The assigned <seealso cref="Microsoft.Xna.Framework.GraphicsDeviceManager"/>.
        /// </summary>
        private GraphicsDeviceManager graphicsDeviceManager;

        /// <summary>
        ///     The used <seealso cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/>.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Engine"/> class.
        /// </summary>
        private Engine(EngineConfig config) : base()
        {
            this.graphicsDeviceManager = new GraphicsDeviceManager(this);

            this.Content.RootDirectory = config.ContentRoot;
        }

        /// <summary>
        ///     The active instance of the <see cref="Engine"/>.
        ///     This is null if the Engine is not running.
        /// </summary>
        public static Engine Instance { get; private set; }

        /// <summary>
        ///     Indicates whether the Engine is running.
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                return Engine.Instance?.IsActive ?? false;
            }
        }

        /// <summary>
        ///     The assigned <seealso cref="Microsoft.Xna.Framework.GraphicsDeviceManager"/>.
        /// </summary>
        public static GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return Engine.Instance?.graphicsDeviceManager;
            }
        }

        /// <summary>
        ///     The used <seealso cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/>.
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get
            {
                return Engine.Instance?.spriteBatch;
            }
        }

        /// <summary>
        ///     Starts the Engine.
        /// </summary>
        public static void Start()
        {
            Engine.Start(EngineConfig.Default);
        }
        
        /// <summary>
        ///     Starts the Engine.
        /// </summary>
        /// <param name="config">The used configuration</param>
        public static void Start(EngineConfig config)
        {
            using (Engine.Instance = new Engine(config))
            {
                Engine.Instance.Run();
            }

            Engine.Instance = null;
        }

        protected override void Initialize()
        {
            // Initialization

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            // Load via this.Content
        }

        protected override void UnloadContent()
        {
            // Non-content Manager
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
