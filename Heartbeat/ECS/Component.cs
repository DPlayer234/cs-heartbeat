using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    public abstract class Component : ECSObject
    {
        public Entity Entity { get; internal set; }

        public bool IsAttachedToEntity
        {
            get
            {
                return this.Entity != null;
            }
        }

        public T GetComponent<T>() where T : Component
        {
            return this.Entity.GetComponent<T>();
        }

        public List<T> GetComponents<T>() where T : Component
        {
            return this.Entity.GetComponents<T>();
        }

        public T GetAnyComponent<T>() where T : Component
        {
            return this.Entity.GetAnyComponent<T>();
        }

        public List<T> GetAnyComponents<T>() where T : Component
        {
            return this.Entity.GetAnyComponents<T>();
        }

        public override void Destroy()
        {
            base.Destroy();

            this.Entity.Components.ContainsMarkedItems = true;
            this.ECS.Components.ContainsMarkedItems = true;
        }

        internal void AttachToEntity(Entity entity)
        {
            this.Entity = entity;
            this.ECS = entity.ECS;

            this.Initialize();
        }
    }
}
