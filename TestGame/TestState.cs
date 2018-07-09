using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heartbeat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame
{
    public class TestEntity : Entity, IUpdateObject
    {
        public int Frames = 0;

        public void Update()
        {
            if (++this.Frames > 600)
            {
                this.Destroy();
            }
        }

        public void LateUpdate()
        {
            // Do nothing
        }
    }

    public class TestComponent : Component, IDrawObject
    {
        public void Draw()
        {
            Engine.SpriteBatch.DrawString(
                TestState.Font,
                (this.Entity as TestEntity)?.Frames.ToString(),
                new Vector2(10, 10),
                Color.Black);
        }
    }

    public class TestState : GameState
    {
        public static SpriteFont Font;

        public static ContentManager Content;

        public override void Initialize()
        {
            TestState.Font = Engine.Game.Content.Load<SpriteFont>("SpriteFont");

            var entity = this.ECS.AddEntity(new TestEntity());

            var component = entity.AddComponent(new TestComponent());
        }
    }
}
