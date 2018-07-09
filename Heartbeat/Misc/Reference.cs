using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    /// <summary>
    ///     A reference type data structure that can hold exactly one value,
    ///     which it can be implicitly converted to.
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    public class Reference<T>
    {
        /// <summary> The held value </summary>
        public T Value;

        /// <summary>
        ///     Converts a reference to its value.
        /// </summary>
        /// <param name="reference">The reference</param>
        public static explicit operator T(Reference<T> reference)
        {
            return reference.Value;
        }

        /// <summary>
        ///     Converts a value to its reference.
        /// </summary>
        /// <param name="value">The value</param>
        public static explicit operator Reference<T>(T value)
        {
            return new Reference<T>()
            {
                Value = value
            };
        }
    }
}
