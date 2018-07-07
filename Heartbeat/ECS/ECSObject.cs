using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    /// <summary>
    ///     Base class for all objects in an <see cref="Heartbeat.ECS.EntityComponentSystem"/>
    /// </summary>
    public abstract class ECSObject
    {
        /// <summary> The ECS this object is attached to. </summary>
        public ECS ECS { get; private set; }

        /// <summary> Indicates whether this object was marked for destruction. </summary>
        public bool MarkedForDestruction { get; protected set; }

        /// <summary> Indicates whether this object was destroyed and may not be used anymore. </summary>
        public bool IsDestroyed { get; protected set; }

        /// <summary>
        ///     Marks the object for destruction and does some pre-clearing.
        /// </summary>
        public virtual void Destroy()
        {
            this.MarkedForDestruction = true;
        }

        /// <summary>
        ///     Called when the object is finally destroyed.
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        ///     Truly destroys the object by calling <seealso cref="OnDestroy"/> and setting <seealso cref="IsDestroyed"/>.
        /// </summary>
        internal void TrulyDestroy()
        {
            this.IsDestroyed = true;

            this.OnDestroy();
        }

        /// <summary>
        ///     Override to have it initialize the object.
        /// </summary>
        protected internal virtual void Initialize() { }
    }
}
