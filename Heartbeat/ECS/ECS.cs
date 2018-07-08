using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace Heartbeat
{
    /// <summary>
    ///     An Entity Component System.
    /// </summary>
    public sealed class ECS
    {
        internal ClassedStorage<Entity> Entities = new ClassedStorage<Entity>();

        internal ClassedStorage<Component> Components = new ClassedStorage<Component>();
        
        internal void Initialize()
        {
            this.PhysicWorld = this.GameState.PhysicWorld;
        }

        public GameState GameState { get; internal set; }

        public World PhysicWorld { get; private set; }

        public T AddEntity<T>(T entity) where T : Entity
        {
            this.Entities.Add(entity);

            entity.Initialize();

            return entity;
        }

        public void Update()
        {
            this.Entities.UpdateAll();
            this.Components.UpdateAll();
        }

        public void LateUpdate()
        {
            this.Entities.LateUpdateAll();
            this.Components.LateUpdateAll();
        }

        public void Draw()
        {
            this.Components.DrawAll();
            this.Entities.DrawAll();
        }

        internal void TrulyDestroy()
        {
            this.Components.DestroyAll();
            this.Entities.DestroyAll();
        }
    }
}
