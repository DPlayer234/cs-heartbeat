using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    public class Entity : ECSObject
    {
        internal ClassedStorage<Component> Components = new ClassedStorage<Component>();

        public bool IsAttachedToECS
        {
            get
            {
                return this.ECS != null;
            }
        }

        public T AddComponent<T>(T component) where T : Component
        {
            this.Components.Add(component);

            component.Initialize();

            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            return this.Components.GetFirstExact<T>();
        }

        public List<T> GetComponents<T>() where T : Component
        {
            return this.Components.GetAllExact<T>();
        }

        public T GetAnyComponent<T>() where T : Component
        {
            return this.Components.GetFirstAny<T>();
        }

        public List<T> GetAnyComponents<T>() where T : Component
        {
            return this.Components.GetAllAny<T>();
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
