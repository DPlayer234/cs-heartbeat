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
        private Dictionary<Type, TypeList> typeDict = new Dictionary<Type, TypeList>();

        /// <summary>
        ///     Adds an item to the storage.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <exception cref="ArgumentNullException">The given item is null</exception>
        public void Add<TSub>(TSub item) where TSub : class, TBase
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            this.GetTypeList<TSub>().Main.Add(item);
        }

        /// <summary>
        ///     Gets the first element of exactly <typeparamref name="TSub"/>
        /// </summary>
        /// <typeparam name="TSub">The type to get an element of</typeparam>
        /// <returns>An element or null if none is found</returns>
        public TSub GetFirstExact<TSub>() where TSub : class, TBase
        {
            return this.GetTypeList<TSub>().GetFirstExact<TSub>();
        }

        /// <summary>
        ///     Gets the first element of <typeparamref name="TSub"/> or any child-types.
        /// </summary>
        /// <typeparam name="TSub">The type to get an element of</typeparam>
        /// <returns>An element or null if none is found</returns>
        public TSub GetFirstAny<TSub>() where TSub : class, TBase
        {
            return this.GetTypeList<TSub>().GetFirstAny<TSub>();
        }

        /// <summary>
        ///     Gets all elements of an exactly <typeparamref name="TSub"/>
        /// </summary>
        /// <typeparam name="TSub">The type to get elements of</typeparam>
        /// <returns>A list of all given elements</returns>
        public List<TSub> GetAllExact<TSub>() where TSub : class, TBase
        {
            return this.GetTypeList<TSub>().GetAllExact<TSub>();
        }

        /// <summary>
        ///     Gets all elements of <typeparamref name="TSub"/> or any child-types.
        /// </summary>
        /// <typeparam name="TSub">The type to get elements of</typeparam>
        /// <returns>A list of all given elements</returns>
        public List<TSub> GetAllAny<TSub>() where TSub : class, TBase
        {
            return this.GetTypeList<TSub>().GetAllAny<TSub>();
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
        protected TypeList GetTypeList<TSub>() where TSub : class, TBase
        {
            Type thisType = typeof(TSub);

            if (!this.typeDict.ContainsKey(thisType))
            {
                TypeList typeList = TypeList.New<TSub>();

                foreach (KeyValuePair<Type, TypeList> item in this.typeDict)
                {
                    TypeList otherList = item.Value;

                    if (otherList.Type.IsSubclassOf(thisType))
                    {
                        typeList.Subs.Add(otherList.Main);
                    }
                    else if (thisType.IsSubclassOf(otherList.Type))
                    {
                        otherList.Subs.Add(typeList.Main);
                    }
                }

                this.storage.Add(typeList.Main);
                this.typeDict.Add(thisType, typeList);

                return typeList;
            }

            return this.typeDict[thisType];
        }

        /// <summary>
        ///     Iterates over all stored items in the storage.
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        ///     A data structure containing a list for a specific type and ones
        ///     for its sub-types.
        /// </summary>
        protected class TypeList
        {
            /// <summary> The type </summary>
            public readonly Type Type;

            /// <summary> The main list </summary>
            public readonly IList Main;

            /// <summary> The lists for sub-types of <see cref="Type"/> </summary>
            public readonly List<IList> Subs = new List<IList>();

            /// <summary>
            ///     Initializes a new instance of the <see cref="TypeList"/> class.
            /// </summary>
            /// <param name="type">The contained type</param>
            /// <param name="main">An empty main list</param>
            private TypeList(Type type, IList main)
            {
                this.Type = type;
                this.Main = main;
            }

            /// <summary>
            ///     Creates a new <see cref="TypeList"/>.
            /// </summary>
            /// <typeparam name="TSub">The contained type.</typeparam>
            /// <returns>A new <see cref="TypeList"/></returns>
            public static TypeList New<TSub>() where TSub : class
            {
                return new TypeList(typeof(TSub), new List<TSub>());
            }

            /// <summary>
            ///     Gets the first element of the exact stored type.
            /// </summary>
            /// <typeparam name="TSub">=<see cref="Type"/></typeparam>
            public TSub GetFirstExact<TSub>() where TSub : class
            {
                return this.Main.Count > 0 ? this.Main[0] as TSub : default(TSub);
            }

            /// <summary>
            ///     Gets the first element.
            /// </summary>
            /// <typeparam name="TSub">=<see cref="Type"/></typeparam>
            public TSub GetFirstAny<TSub>() where TSub : class
            {
                if (this.Main.Count > 0)
                {
                    return this.Main[0] as TSub;
                }

                for (int i = 0; i < this.Subs.Count; i++)
                {
                    IList sub = this.Subs[i];

                    if (sub.Count > 0)
                    {
                        return sub[0] as TSub;
                    }
                }

                return default(TSub);
            }

            /// <summary>
            ///     Gets all elements of the exact stored type.
            /// </summary>
            /// <typeparam name="TSub">=<see cref="Type"/></typeparam>
            public List<TSub> GetAllExact<TSub>()
            {
                return new List<TSub>(this.Main as List<TSub>);
            }

            /// <summary>
            ///     Gets all elements.
            /// </summary>
            /// <typeparam name="TSub">=<see cref="Type"/></typeparam>
            public List<TSub> GetAllAny<TSub>()
            {
                List<TSub> subs = this.GetAllExact<TSub>();

                for (int i = 0; i < this.Subs.Count; i++)
                {
                    foreach (TSub item in this.Subs[i])
                    {
                        subs.Add(item);
                    }
                }

                return subs;
            }
        }
    }
}
