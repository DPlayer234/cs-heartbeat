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
        public GameState()
        {
            this.PhysicWorld = new World(new Vector2(0.0f, 9.85f), true);

            this.ECS = new ECS();

            this.ECS.GameState = this;

            this.ECS.Initialize();
        }

        public float DeltaTime { get; protected internal set; }

        public World PhysicWorld { get; private set; }

        public ECS ECS { get; private set; }
    }
}
