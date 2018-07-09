using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heartbeat
{
    /// <summary>
    ///     Supplies extensions for lists and similar classes.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        ///     Returns the first element in the list or the default value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list</typeparam>
        /// <param name="self">The list to get the first item of</param>
        /// <returns>The first item or the default value/returns>
        public static T FirstSafe<T>(this IList<T> self)
        {
            return self.Count > 0 ? self[0] : default(T);
        }

        /// <summary>
        ///     Turns a collection into a string.
        /// </summary>
        /// <param name="self">The collection in question</param>
        /// <param name="separator">The separator between the elements</param>
        /// <returns>A string version</returns>
        public static string ToStringCollection(this ICollection self, string separator = ", ")
        {
            string output = string.Empty;

            foreach (object item in self)
            {
                output += item + separator;
            }

            return output;
        }
    }
}
