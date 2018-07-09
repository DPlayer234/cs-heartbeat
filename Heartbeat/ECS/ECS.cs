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
        internal ECSStorage<Entity> Entities = new ECSStorage<Entity>();

        internal ECSStorage<Component> Components = new ECSStorage<Component>();
        
        internal void Initialize()
        {
            this.PhysicWorld = this.GameState.PhysicWorld;
        }

        public GameState GameState { get; internal set; }

        public World PhysicWorld { get; private set; }

        public T AddEntity<T>(T entity) where T : Entity
        {
            this.Entities.Add(entity);
            entity.AttachToECS(this);

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

        public void DestroyMarkedItems()
        {
            this.Entities.DestroyMarkedItems();
            this.Components.DestroyMarkedItems();
        }

        public void DestroyAll()
        {
            this.Components.DestroyAll();
            this.Entities.DestroyAll();
        }

        internal void TrulyDestroy()
        {
            this.DestroyAll();
            this.DestroyMarkedItems();
        }
    }
}
