namespace VehicleCostsMonitor.Web.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class CollectionExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable<out T>
        /// </summary>
        /// <param name="action">
        /// The System.Action delegate to perform on each element of the System.Collections.Generic.IEnumerable<out T>
        /// </param>
        /// <returns>
        /// System.Collections.Generic.IEnumerable<T>
        /// </returns>
        /// <exception>
        /// System.ArgumentNullException
        /// </exception>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection can not be null");
            }

            if (action == null)
            {
                throw new ArgumentNullException("Action can not be null");
            }

            foreach (var item in collection)
            {
                action(item);
            }

            return collection;
        }
    }
}
