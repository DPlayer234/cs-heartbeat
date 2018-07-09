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
    /// <typeparam name="TBase">The base type for stored objects</typeparam>
    internal class ClassedStorage<TBase> : IEnumerable<TBase> where TBase : class
    {
        /// <summary>
        ///     Stores all lists of items.
        /// </summary>
        protected List<IList> storage = new List<IList>();

        /// <summary>
        ///     Assigns the types a list.
        /// </summary>
        private Dictionary<Type, IList> typeDict = new Dictionary<Type, IList>();

        /// <summary>
        ///     Adds an item to the storage.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <exception cref="ArgumentNullException">The given item is null</exception>
        public void Add<TSub>(TSub item) where TSub : class, TBase
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            this.GetTypeList<TSub>().Add(item);
        }

        /// <summary>
        ///     Gets the first element of exactly <typeparamref name="TSub"/>
        /// </summary>
        /// <typeparam name="TSub">The type to get an element of</typeparam>
        /// <returns>An element or null if none is found</returns>
        public TSub GetFirstExact<TSub>() where TSub : class, TBase
        {
            return this.GetTypeList<TSub>().FirstSafe();
        }

        /// <summary>
        ///     Gets the first element of <typeparamref name="TSub"/> or any child-types.
        /// </summary>
        /// <typeparam name="TSub">The type to get an element of</typeparam>
        /// <returns>An element or null if none is found</returns>
        public TSub GetFirstAny<TSub>() where TSub : class, TBase
        {
            for (int i = 0; i < this.storage.Count; i++)
            {
                IList list = this.storage[i];

                if (list.Count > 0 && list[0] is TSub)
                {
                    return list[0] as TSub;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets all elements of an exactly <typeparamref name="TSub"/>
        /// </summary>
        /// <typeparam name="TSub">The type to get elements of</typeparam>
        /// <returns>A list of all given elements</returns>
        public List<TSub> GetAllExact<TSub>() where TSub : class, TBase
        {
            return new List<TSub>(this.GetTypeList<TSub>());
        }

        /// <summary>
        ///     Gets all elements of <typeparamref name="TSub"/> or any child-types.
        /// </summary>
        /// <typeparam name="TSub">The type to get elements of</typeparam>
        /// <returns>A list of all given elements</returns>
        public List<TSub> GetAllAny<TSub>() where TSub : class, TBase
        {
            List<TSub> subs = new List<TSub>();

            for (int i = 0; i < this.storage.Count; i++)
            {
                IList list = this.storage[i];

                if (list.Count > 0 && list[0] is TSub)
                {
                    foreach (TSub item in list)
                    {
                        subs.Add(item);
                    }
                }
            }

            return subs;
        }

        /// <summary>
        ///     Iterates over all stored items in the storage.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<TBase> GetEnumerator()
        {
            foreach (IList list in this.storage)
            {
                foreach (TBase item in list)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        ///     Gets the list of a specified type.
        ///     May create a new one.
        /// </summary>
        /// <param name="specType">The type to get the list of</param>
        /// <returns>A list</returns>
        protected List<TSub> GetTypeList<TSub>() where TSub : class, TBase
        {
            Type specType = typeof(TSub);

            if (!this.typeDict.ContainsKey(specType))
            {
                List<TSub> list = new List<TSub>();

                this.storage.Add(list);

                this.typeDict.Add(specType, list);

                return list;
            }

            return this.typeDict[specType] as List<TSub>;
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
