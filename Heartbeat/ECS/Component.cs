using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    public class Component : ECSObject
    {
        public Entity Entity { get; private set; }

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
    }
}
