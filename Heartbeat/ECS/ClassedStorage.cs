using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    /// <summary>
    ///     This classed storage is designed to allow quicker access to objects of
    ///     a certain type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ClassedStorage<T> : IEnumerable<T> where T : ECSObject
    {
        /// <summary>
        ///     Assigns the types to the index of a list.
        /// </summary>
        private Dictionary<Type, int> types = new Dictionary<Type, int>();

        /// <summary>
        ///     Stores all lists of items.
        /// </summary>
        private List<List<T>> storage = new List<List<T>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassedStorage{T}"/> class.
        /// </summary>
        public ClassedStorage()
        {
            this.ContainsMarkedItems = false;
        }

        /// <summary>
        ///     Indicates whether items queued for destruction are contained.
        /// </summary>
        public bool ContainsMarkedItems { get; internal set; }

        /// <summary>
        ///     Updates all stored items.
        /// </summary>
        public void UpdateAll()
        {
            for (int si = 0; si < this.storage.Count; si++)
            {
                List<T> list = this.storage[si];

                if (list.Count == 0) continue;
                if (!(list[0] is IUpdatable)) continue;

                for (int li = 0; li < list.Count; li++)
                {
                    (list[li] as IUpdatable).Update();
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
                List<T> list = this.storage[si];

                if (list.Count == 0) continue;
                if (!(list[0] is IUpdatable)) continue;

                for (int li = 0; li < list.Count; li++)
                {
                    (list[li] as IUpdatable).LateUpdate();
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
                List<T> list = this.storage[si];

                if (list.Count == 0) continue;
                if (!(list[0] is IDrawable)) continue;

                for (int li = 0; li < list.Count; li++)
                {
                    (list[li] as IDrawable).Draw();
                }
            }
        }

        /// <summary>
        ///     Adds an item to the storage.
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            this.GetTypeList(item.GetType()).Add(item);
        }

        /// <summary>
        ///     Gets the first element of exactly <typeparamref name="TSub"/>
        /// </summary>
        /// <typeparam name="TSub">The type to get an element of</typeparam>
        /// <returns>An element or null if none is found</returns>
        public TSub GetFirstExact<TSub>() where TSub : T
        {
            return this.GetTypeList(typeof(TSub)).FirstOrDefault() as TSub;
        }

        /// <summary>
        ///     Gets the first element of <typeparamref name="TSub"/> or any child-types.
        /// </summary>
        /// <typeparam name="TSub">The type to get an element of</typeparam>
        /// <returns>An element or null if none is found</returns>
        public TSub GetFirstAny<TSub>() where TSub : T
        {
            foreach (KeyValuePair<Type, int> data in this.types)
            {
                List<T> list = this.storage[data.Value];

                if (list.Count > 0 && data.Key.IsSubclassOf(typeof(TSub)))
                {
                    return list.First() as TSub;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets all elements of an exactly <typeparamref name="TSub"/>
        /// </summary>
        /// <typeparam name="TSub">The type to get elements of</typeparam>
        /// <returns>A list of all given elements</returns>
        public List<TSub> GetAllExact<TSub>() where TSub : T
        {
            List<TSub> subs = new List<TSub>();

            var list = this.GetTypeList(typeof(TSub));

            foreach (T item in list)
            {
                subs.Add(item as TSub);
            }

            return subs;
        }

        /// <summary>
        ///     Gets all elements of <typeparamref name="TSub"/> or any child-types.
        /// </summary>
        /// <typeparam name="TSub">The type to get elements of</typeparam>
        /// <returns>A list of all given elements</returns>
        public List<TSub> GetAllAny<TSub>() where TSub : T
        {
            List<TSub> subs = new List<TSub>();

            foreach (KeyValuePair<Type, int> data in this.types)
            {
                List<T> list = this.storage[data.Value];

                if (list.Count > 0 && data.Key.IsSubclassOf(typeof(TSub)))
                {
                    foreach (T item in list)
                    {
                        subs.Add(item as TSub);
                    }
                }
            }

            return subs;
        }

        /// <summary>
        ///     Iterates over all stored items in the storage.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (List<T> list in this.storage)
            {
                foreach (T item in list)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        ///     Destroys all items.
        /// </summary>
        internal void DestroyAll()
        {
            for (int si = 0; si < this.storage.Count; si++)
            {
                List<T> list = this.storage[si];

                for (int li = list.Count - 1; li >= 0; li--)
                {
                    T item = list[li];
                    
                    item.TrulyDestroy();
                    list.RemoveAt(li);
                }
            }

            this.ContainsMarkedItems = false;
        }

        /// <summary>
        ///     Gets the list of a specified type.
        ///     May create a new one.
        /// </summary>
        /// <param name="specType">The type to get the list of</param>
        /// <returns>A list</returns>
        private List<T> GetTypeList(Type specType)
        {
            if (!this.types.ContainsKey(specType))
            {
                List<T> list = new List<T>();

                this.storage.Add(list);

                this.types.Add(specType, this.storage.Count - 1);

                return list;
            }

            return this.storage[this.types[specType]];
        }

        /// <summary>
        ///     Destroys and removes all items marked for destruction.
        /// </summary>
        private void DestroyMarkedItems()
        {
            if (!this.ContainsMarkedItems) return;

            this.ContainsMarkedItems = false;

            for (int si = 0; si < this.storage.Count; si++)
            {
                List<T> list = this.storage[si];

                for (int li = list.Count - 1; li >= 0; li--)
                {
                    T item = list[li];

                    if (item.MarkedForDestruction)
                    {
                        item.TrulyDestroy();
                        list.RemoveAt(li);
                    }
                }
            }
        }

        /// <summary>
        ///     Iterates over all stored items in the storage.
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
