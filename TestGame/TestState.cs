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
    public class TestEntity : Entity, IUpdateObject, IDrawObject
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

        public void Draw()
        {
            var print =
                (this.GetComponent<TestComponent>()?.ToString() ?? "null") + " E\n" +
                (this.GetAnyComponent<TestComponent>()?.ToString() ?? "null") + " A\n" +
                (this.GetComponent<TestComponentSub>()?.ToString() ?? "null") + " ES\n" +
                (this.GetAnyComponent<TestComponentSub>()?.ToString() ?? "null") + " AS\n" +
                (this.GetComponents<TestComponent>()?.ToStringCollection() ?? "null") + " EL\n" +
                (this.GetAnyComponents<TestComponent>()?.ToStringCollection() ?? "null") + " AL\n" +
                (this.GetComponents<TestComponentSub>()?.ToStringCollection() ?? "null") + " ESL\n" +
                (this.GetAnyComponents<TestComponentSub>()?.ToStringCollection() ?? "null") + " ASL";

            Engine.SpriteBatch.DrawString(
                TestState.Font,
                print,
                new Vector2(10, 30),
                Color.Black);
        }
    }

    public class TestComponent : Component, IDrawObject
    {
        public void Draw()
        {
            Engine.SpriteBatch.DrawString(
                TestState.Font,
                (this.Entity as TestEntity)?.Frames.ToString() ?? "null",
                new Vector2(10, 10),
                Color.Black);
        }
    }

    public class TestComponentSub : TestComponent { }

    public class TestState : GameState
    {
        public static SpriteFont Font;

        public static ContentManager Content;

        public override void Initialize()
        {
            TestState.Font = Engine.Game.Content.Load<SpriteFont>("SpriteFont");

            var entity = this.ECS.AddEntity(new TestEntity());

            entity.AddComponent(new TestComponentSub());
            entity.AddComponent(new TestComponent());
        }
    }
}
