using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace Heartbeat
{
    public class GameState
    {
        public int VelocityIterations = 8;

        public int PositionIteration = 3;

        public float TimeScale = 1.0f;

        public GameState()
        {
            this.PhysicWorld = new World(new Vector2(0.0f, 9.85f), true);

            this.ECS = new ECS();

            this.ECS.GameState = this;

            this.ECS.Initialize();
        }

        public float UnscaledDeltaTime { get; protected internal set; }

        public float DeltaTime { get; private set; }

        public World PhysicWorld { get; private set; }

        public ECS ECS { get; private set; }

        public EngineInstance Engine { get; internal set; }

        public bool IsActive
        {
            get
            {
                return this.Engine.ActiveGameState == this;
            }
        }

        public virtual void Initialize() { }

        public virtual void Update()
        {
            this.DeltaTime = this.UnscaledDeltaTime * this.TimeScale;

            this.ECS.Update();

            this.PhysicWorld.Step(this.DeltaTime, this.VelocityIterations, this.PositionIteration);

            this.ECS.LateUpdate();
        }

        public virtual void Draw()
        {
            this.ECS.Draw();
        }

        public virtual void OnPause() { }

        public virtual void OnResume() { }

        public virtual void OnDestroy() { }

        internal void TrulyDestroy()
        {
            this.ECS.TrulyDestroy();

            this.OnDestroy();
        }
    }
}
