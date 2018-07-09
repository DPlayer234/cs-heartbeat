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

            this.Entity.Components.Add(this);
            this.ECS.Components.Add(this);

            this.Initialize();
        }
    }
}
