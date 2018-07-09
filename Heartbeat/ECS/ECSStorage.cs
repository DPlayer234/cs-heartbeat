using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Heartbeat
{
    /// <summary>
    ///     This is used to store any amount of <seealso cref="ECSObject"/>s.
    /// </summary>
    /// <typeparam name="TBase">The base type for stored objects</typeparam>
    internal sealed class ECSStorage<TBase> : ClassedStorage<TBase> where TBase : ECSObject
    {
        /// <summary>
        ///     Indicates whether items queued for destruction are contained.
        /// </summary>
        public bool ContainsMarkedItems { get; internal set; } = false;

        /// <summary>
        ///     Updates all stored items.
        /// </summary>
        public void UpdateAll()
        {
            for (int si = 0; si < this.storage.Count; si++)
            {
                IList list = this.storage[si];

                if (list.Count == 0 || !(list[0] is IUpdateObject)) continue;

                for (int li = 0; li < list.Count; li++)
                {
                    (list[li] as IUpdateObject).Update();
                }
            }
        }

        /// <summary>
        ///     Late Updates all stored items.
        /// </summary>
        public void LateUpdateAll()
        {
            for (int si = 0; si < this.storage.Count; si++)
            {
                IList list = this.storage[si];

                if (list.Count == 0 || !(list[0] is IUpdateObject)) continue;

                for (int li = 0; li < list.Count; li++)
                {
                    (list[li] as IUpdateObject).LateUpdate();
                }
            }
        }

        /// <summary>
        ///     Draws all stored items.
        /// </summary>
        public void DrawAll()
        {
            for (int si = 0; si < this.storage.Count; si++)
            {
                IList list = this.storage[si];
                
                if (list.Count == 0 || !(list[0] is IDrawObject)) continue;

                for (int li = 0; li < list.Count; li++)
                {
                    (list[li] as IDrawObject).Draw();
                }
            }
        }

        /// <summary>
        ///     Destroys and removes all items marked for destruction.
        /// </summary>
        public void DestroyMarkedItems()
        {
            if (!this.ContainsMarkedItems) return;

            this.ContainsMarkedItems = false;

            for (int si = 0; si < this.storage.Count; si++)
            {
                IList list = this.storage[si];

                for (int li = list.Count - 1; li >= 0; li--)
                {
                    TBase item = list[li] as TBase;

                    if (item.MarkedForDestruction)
                    {
                        item.TrulyDestroy();
                        list.RemoveAt(li);
                    }
                }
            }
        }

        /// <summary>
        ///     Calls destroy for every item.
        /// </summary>
        public void DestroyAll()
        {
            for (int si = 0; si < this.storage.Count; si++)
            {
                IList list = this.storage[si];

                for (int li = list.Count - 1; li >= 0; li--)
                {
                    TBase item = list[li] as TBase;

                    item.Destroy();
                }
            }
        }

        /// <summary>
        ///     Destroys all items.
        /// </summary>
        internal void TrulyDestroyAll()
        {
            for (int si = 0; si < this.storage.Count; si++)
            {
                IList list = this.storage[si];

                for (int li = list.Count - 1; li >= 0; li--)
                {
                    TBase item = list[li] as TBase;

                    item.TrulyDestroy();
                    list.RemoveAt(li);
                }
            }

            this.ContainsMarkedItems = false;
        }
    }
}
