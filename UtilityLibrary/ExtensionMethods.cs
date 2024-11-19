using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains extension utility methods.
    /// </summary>
    public static class ExtensionMethods
    {
        private static class NullChecker<T> where T : class
        {
            private static readonly List<Func<T, bool>> m_Checkers;
            private static readonly List<string> m_Names;

            static NullChecker()
            {
                m_Checkers = new List<Func<T, bool>>();
                m_Names = new List<string>();

                var type = typeof(T);
                var parameters = from p in type.GetConstructors()[0].GetParameters()
                                 select p.Name;
                foreach (string name in parameters)
                {
                    m_Names.Add(name);
                    PropertyInfo property = type.GetProperty(name);

                    if (property.PropertyType.IsValueType)
                    {
                        throw new ArgumentException("Property " + property + " is a value type.");
                    }

                    var parameter = Expression.Parameter(type, "container");
                    var propertyAccess = Expression.Property(parameter, property);
                    var nullValue = Expression.Constant(null, property.PropertyType);
                    var equality = Expression.Equal(propertyAccess, nullValue);
                    var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);
                    m_Checkers.Add(lambda.Compile());
                }
            }

            internal static void Check(T item)
            {
                for (int i = 0; i < m_Checkers.Count; i++)
                {
                    if (m_Checkers[i](item))
                    {
                        throw new ArgumentNullException(m_Names[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Raises an event in a thread-safe manner.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> raising the event.</param>
        /// <param name="handler">The <see cref="System.EventHandler"/> to be invoked.</param>
        /// <param name="eventLock">The <see cref="System.Object"/> controlling access to the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> to be passed to event listeners.</param>
        public static void Raise(this object sender, ref EventHandler handler, object eventLock, EventArgs e)
        {
            EventHandler handlerCopy;
            lock (eventLock)
            {
                handlerCopy = handler;
            }

            if (handlerCopy != null)
            {
                handlerCopy(sender, e);
            }
        }

        /// <summary>
        /// Raises an event in a thread-safe manner.
        /// </summary>
        /// <typeparam name="T">The type parameter of the event.</typeparam>
        /// <param name="sender">The <see cref="System.Object"/> raising the event.</param>
        /// <param name="handler">The <see cref="System.EventHandler{T}"/> to be invoked.</param>
        /// <param name="eventLock">The <see cref="System.Object"/> controlling access to the event.</param>
        /// <param name="e">The <typeparamref name="T"/> instance to be passed to event listeners.</param>
        public static void Raise<T>(this object sender, ref EventHandler<T> handler, object eventLock, T e) where T : EventArgs
        {
            EventHandler<T> handlerCopy;
            lock (eventLock)
            {
                handlerCopy = handler;
            }

            if (handlerCopy != null)
            {
                handlerCopy(sender, e);
            }
        }

        /// <summary>
        /// Retrieves a value from the <see cref="System.Runtime.Serialization.SerializationInfo"/> store.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> instance.</param>
        /// <param name="name">The name associated with the value to retrieve.</param>
        /// <returns>The object of the specified type associated with <paramref name="name"/>.</returns>
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            var type = typeof(T);
            return (T)info.GetValue(name, type);
        }

        /// <summary>
        /// Adds a value into the <see cref="System.Runtime.Serialization.SerializationInfo"/> store, where <paramref name="value"/> is associated with <paramref name="name"/> and is serialized as being of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to add.</typeparam>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> instance.</param>
        /// <param name="name">The name to associate with the value, so it can be deserialized later.</param>
        /// <param name="value">The value to be serialized. Any children of this object will automatically be serialized.</param>
        public static void AddValue<T>(this SerializationInfo info, string name, T value)
        {
            info.AddValue(name, value, typeof(T));
        }

        /// <summary>
        /// Generates a copy of an existing <see cref="System.Collections.Generic.IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">An <see cref="System.Collections.Generic.IEnumerable{TSource}"/> whose elements are to be used.</param>
        /// <returns>An enumerated copy of <paramref name="source"/>.</returns>
        public static IEnumerable<TSource> Enumerate<TSource>(this IEnumerable<TSource> source)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Generates an <see cref="System.Collections.Generic.IEnumerable{TSource}"/> for a single element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The item to encapsulate.</param>
        /// <returns>A sequence containing <paramref name="source"/>.</returns>
        public static IEnumerable<TSource> EnumerateSingle<TSource>(this TSource source)
        {
            yield return source;
        }

        /// <summary>
        /// Checks for null parameters specified in an anonymously typed container and throws exceptions as appropriate.
        /// </summary>
        /// <typeparam name="T">The anonymous type of the container.</typeparam>
        /// <param name="container">The parameter container.</param>
        public static void CheckNotNull<T>(this T container) where T : class
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            NullChecker<T>.Check(container);
        }

        /// <summary>
        /// Returns a normally distributed random number with mean 0 and standard deviation 1.
        /// </summary>
        /// <param name="generator">A <see cref="System.Random"/> to use.</param>
        /// <returns>A normally distributed random number.</returns>
        public static double NextNormal(this Random generator)
        {
            double u1 = generator.NextDouble();
            double u2 = generator.NextDouble();
            return Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }

        /// <summary>
        /// Returns a chi-square distributed random number with <paramref name="k"/> degrees of freedom.
        /// </summary>
        /// <param name="generator">A <see cref="System.Random"/> to use.</param>
        /// <param name="k">The number of degrees of freedom for the chi-square distribution.</param>
        /// <returns>A chi-square distributed random number.</returns>
        public static double NextChiSquare(this Random generator, int k)
        {
            if (k <= 0)
            {
                throw new ArgumentOutOfRangeException("k");
            }

            double x = 0;
            for (int i = 0; i < k; i++)
            {
                double u = generator.NextNormal();
                x += u * u;
            }

            return x;
        }

        /// <summary>
        /// Performs an action a given number of times.
        /// </summary>
        /// <param name="number">The number of times to perform the action.</param>
        /// <param name="action">An action to perform.</param>
        public static void Times(this int number, Action action)
        {
            for (int i = 0; i < number; i++)
            {
                action();
            }
        }

        /// <summary>
        /// Gets the cardinality (number of elements) of a given dimension of an array.
        /// </summary>
        /// <param name="array">The array whose cardinality to get.</param>
        /// <param name="dimension">The dimension whose cardinality to get.</param>
        /// <returns>The cardinality of the array at the given dimension.</returns>
        public static int GetCardinality(this Array array, int dimension)
        {
            return array.GetUpperBound(dimension) - array.GetLowerBound(dimension) + 1;
        }

        /// <summary>
        /// Adds the elements of the specified sequence to a set.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the set.</typeparam>
        /// <param name="set">The set to be added to.</param>
        /// <param name="source">A sequence whose elements are to be added to the set.</param>
        public static void Add<T>(this HashSet<T> set, IEnumerable<T> source)
        {
            foreach (T item in source)
            {
                set.Add(item);
            }
        }

        /// <summary>
        /// Produces the set union of two sequences by using a specified <see cref="System.Collections.Generic.IEqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="System.Collections.Generic.IEnumerable{T}"/> whose distinct elements form the first set for the union.</param>
        /// <param name="second">A <typeparamref name="TSource"/> that forms the second set for the union.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> that contains the elements from both input sequences, excluding duplicates.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, TSource second)
        {
            return first.Union(second.EnumerateSingle());
        }

        /// <summary>
        /// Produces the set union of two sequences by using a specified <see cref="System.Collections.Generic.IEqualityComparer{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="System.Collections.Generic.IEnumerable{TSource}"/> whose distinct elements form the first set for the union.</param>
        /// <param name="second">A <typeparamref name="TSource"/> that forms the second set for the union.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IEqualityComparer{TSource}"/> to compare values.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{TSource}"/> that contains the elements from both input sequences, excluding duplicates.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, TSource second, IEqualityComparer<TSource> comparer)
        {
            return first.Union(second.EnumerateSingle(), comparer);
        }

        /// <summary>
        /// Produces the set difference of two sequences by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="System.Collections.Generic.IEnumerable{T}"/> whose elements that are not also in second will be returned.</param>
        /// <param name="second">A <typeparamref name="TSource"/> that, if it occurs in the first sequence, will cause such elements to be removed from the returned sequence.</param>
        /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, TSource second)
        {
            return first.Except(second.EnumerateSingle());
        }

        /// <summary>
        /// Produces the set difference of two sequences by using the specified <see cref="System.Collections.Generic.IEqualityComparer{T}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="System.Collections.Generic.IEnumerable{TSource}"/> whose elements that are not also in second will be returned.</param>
        /// <param name="second">A <typeparamref name="TSource"/> that, if occurring in the first sequence, will cause such elements to be removed from the returned sequence.</param>
        /// <param name="comparer">An <see cref="System.Collections.Generic.IEqualityComparer{TSource}"/> to compare values.</param>
        /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, TSource second, IEqualityComparer<TSource> comparer)
        {
            return first.Except(second.EnumerateSingle(), comparer);
        }

        /// <summary>
        /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="System.Collections.Generic.IEnumerable{TSource}"/> whose distinct elements that also appear in second will be returned.</param>
        /// <param name="second">An <typeparamref name="TSource"/> that, if occurring in the first sequence, will be returned.</param>
        /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, TSource second)
        {
            return first.Intersect(second.EnumerateSingle());
        }
        
        /// <summary>
        /// Produces the set intersection of two sequences by using the specified <see cref="System.Collections.Generic.IEqualityComparer{T}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="System.Collections.Generic.IEnumerable{TSource}"/> whose distinct elements that also appear in second will be returned.</param>
        /// <param name="second">An <typeparamref name="TSource"/> that, if occurring in the first sequence, will be returned.</param>
        /// <param name="comparer">An <see cref="System.Collections.Generic.IEqualityComparer{TSource}"/> to compare values.</param>
        /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, TSource second, IEqualityComparer<TSource> comparer)
        {
            return first.Intersect(second.EnumerateSingle(), comparer);
        }

        /// <summary>
        /// Returns distinct elements from a sequence by using a specified comparison callback to compare values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source"></param>
        /// <param name="comparison"></param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparison)
        {
            return source.Distinct(new FunctionalEqualityComparer<T>(comparison));
        }

        /// <summary>
        /// Calculates the average value of a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to iterate over.</param>
        /// <param name="sum">An associative binary operation that adds two <typeparamref name="T"/> objects.</param>
        /// <param name="quotient">A right-external binary operation that divides a <typeparamref name="T"/> object by a <see cref="System.Int32"/>.</param>
        /// <returns>The averaged value of each element of <paramref name="source"/>.</returns>
        public static T Average<T>(this IEnumerable<T> source, Func<T, T, T> sum, Func<T, int, T> quotient)
        {
            return Average(source, sum, quotient, default(T));
        }

        /// <summary>
        /// Calculates the average value of a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence to iterate over.</param>
        /// <param name="sum">An associative binary operation that adds two <typeparamref name="T"/> objects.</param>
        /// <param name="quotient">A right-external binary operation that divides a <typeparamref name="T"/> object by a <see cref="System.Int32"/>.</param>
        /// <param name="seed">An initial value to be summed from.</param>
        /// <returns>The averaged value of each element of <paramref name="source"/>.</returns>
        public static T Average<T>(this IEnumerable<T> source, Func<T, T, T> sum, Func<T, int, T> quotient, T seed)
        {
            int count = source.Count();
            return quotient(source.FoldLeft(sum, seed), count == 0 ? 1 : count);
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the beginning of the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <typeparam name="TAggregate">The type of the aggregated value.</typeparam>
        /// <typeparam name="TResult">The type of the returned value.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <param name="seed">A seed value for the aggregation.</param>
        /// <param name="transform">A function to be applied to the aggregated value before it is returned.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TResult FoldLeft<TSource, TAggregate, TResult>(this IEnumerable<TSource> source, Func<TAggregate, TSource, TAggregate> aggregate, TAggregate seed, Func<TAggregate, TResult> transform)
        {
            return Utility.FoldLeft(source, aggregate, seed, transform);
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the beginning of the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <typeparam name="TResult">The type of the aggregated value.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <param name="seed">A seed value for the aggregation.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TResult FoldLeft<TSource, TResult>(this IEnumerable<TSource> source, Func<TResult, TSource, TResult> aggregate, TResult seed)
        {
            return Utility.FoldLeft(source, aggregate, seed);
        }

        /// <summary>
        /// Applies a function over a sequence of elements, starting from the beginning of the sequence and using the first list element as a seed.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TSource FoldLeft<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> aggregate)
        {
            return Utility.FoldLeft(source, aggregate);
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the end of the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <typeparam name="TAggregate">The type of the aggregated value.</typeparam>
        /// <typeparam name="TResult">The type of the returned value.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <param name="seed">A seed value for the aggregation.</param>
        /// <param name="transform">A function to be applied to the aggregated value before it is returned.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TResult FoldRight<TSource, TAggregate, TResult>(this IEnumerable<TSource> source, Func<TSource, TAggregate, TAggregate> aggregate, TAggregate seed, Func<TAggregate, TResult> transform)
        {
            return Utility.FoldRight(source, aggregate, seed, transform);
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the end of the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <typeparam name="TResult">The type of the aggregated value.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="f">The function to be applied.</param>
        /// <param name="seed">A seed value for the aggregation.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="f"/>.</returns>
        public static TResult FoldRight<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult, TResult> f, TResult seed)
        {
            return Utility.FoldRight(source, f, seed);
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the end of the sequence and using the final list element as a seed.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the collection.</typeparam>
        /// <param name="source">The collection over which the function is to be applied.</param>
        /// <param name="f">The function to be applied.</param>
        /// <returns>The aggregated value of the collection under <paramref name="f"/>.</returns>
        public static TSource FoldRight<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> f)
        {
            return Utility.FoldRight(source, f);
        }
    }
}