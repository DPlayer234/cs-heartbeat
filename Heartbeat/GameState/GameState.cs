using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace Heartbeat
{
    /// <summary>
    ///     The base class for any state of the game.
    ///     May also be used as is and manually initialized.
    /// </summary>
    public class GameState
    {
        /// <summary> The iterations for the velocity solver of <seealso cref="PhysicWorld"/> </summary>
        public int VelocityIterations = 8;

        /// <summary> The iterations for the position solver of <seealso cref="PhysicWorld"/> </summary>
        public int PositionIteration = 3;

        /// <summary> The multiplier for <seealso cref="DeltaTime"/> </summary>
        public float TimeScale = 1.0f;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        public GameState()
        {
            this.PhysicWorld = new World(new Vector2(0.0f, 9.85f), true);

            this.ECS = new ECS();

            this.ECS.GameState = this;

            this.ECS.Initialize();
        }

        /// <summary> The Box2D <see cref="World"/> used </summary>
        public World PhysicWorld { get; private set; }

        /// <summary> The <see cref="Heartbeat.ECS"/> used for Entities </summary>
        public ECS ECS { get; private set; }

        /// <summary> The "next" game state. This is the one that was pushed directly on top of this one. </summary>
        public GameState NextState { get; internal set; }

        /// <summary> The "last" game state. This is the state that becomes active when this one is popped. </summary>
        public GameState LastState { get; internal set; }

        /// <summary>
        ///     Is this the active GameState?
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Engine.ActiveGameState == this;
            }
        }

        /// <summary> Is the GameState already in use? If so, it cannot be passed to <seealso cref="Engine.PushGameState{T}(T)"/>. </summary>
        public bool IsInUse { get; internal set; }

        /// <summary>
        ///     Override to initialize the GameState when it is pushed.
        ///     You do not need to call this.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        ///     Updates the GameState.
        /// </summary>
        public virtual void Update()
        {
            this.ECS.DestroyMarkedItems();

            this.ECS.Update();

            this.PhysicWorld.Step(Engine.DeltaTime, this.VelocityIterations, this.PositionIteration);

            this.ECS.LateUpdate();
        }

        /// <summary>
        ///     Draws the GameState.
        /// </summary>
        public virtual void Draw()
        {
            this.ECS.Draw();
        }

        /// <summary>
        ///     Called when the GameState is paused by having another state pushed on top.
        /// </summary>
        public virtual void OnPause() { }

        /// <summary>
        ///     Called when the GameState is resumed by becoming the active state again.
        /// </summary>
        public virtual void OnResume() { }

        /// <summary>
        ///     Called when the GameState is destroyed by being popped.
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        ///     Truly destroys all components of the GameState.
        /// </summary>
        internal void TrulyDestroy()
        {
            this.ECS.TrulyDestroy();

            this.OnDestroy();
        }
    }
}
