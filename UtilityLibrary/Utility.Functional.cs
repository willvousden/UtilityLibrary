using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains commonly used properties and methods.
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// Applies a function over a source of items, starting from the beginning of the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the source.</typeparam>
        /// <typeparam name="TAggregate">The type of the aggregated value.</typeparam>
        /// <typeparam name="TResult">The type of the returned value.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <param name="seed">A seed value for the aggregation.</param>
        /// <param name="transform">A function to be applied to the aggregated value before it is returned.</param>
        /// <returns>The aggregated value of the source under <paramref name="aggregate"/>.</returns>
        public static TResult FoldLeft<TSource, TAggregate, TResult>(IEnumerable<TSource> source, Func<TAggregate, TSource, TAggregate> aggregate, TAggregate seed, Func<TAggregate, TResult> transform)
        {
            new { transform }.CheckNotNull();

            return transform(FoldLeft(source, aggregate, seed));
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the beginning of the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <typeparam name="TAggregate">The type of the aggregated value.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <param name="seed">A seed value for the aggregation.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TAggregate FoldLeft<TSource, TAggregate>(IEnumerable<TSource> source, Func<TAggregate, TSource, TAggregate> aggregate, TAggregate seed)
        {
            new { source, aggregate }.CheckNotNull();

            TAggregate accumulator = seed;
            foreach (TSource item in source)
            {
                accumulator = aggregate(accumulator, item);
            }

            return accumulator;
        }

        /// <summary>
        /// Applies a function over a sequence of elements, starting from the beginning of the sequence and using the first list element as a seed.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TSource FoldLeft<TSource>(IEnumerable<TSource> source, Func<TSource, TSource, TSource> aggregate)
        {
            new { source, aggregate }.CheckNotNull();

            TSource accumulator = source.First();
            source = source.Skip(1);
            foreach (TSource item in source)
            {
                accumulator = aggregate(accumulator, item);
            }

            return accumulator;
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
        public static TResult FoldRight<TSource, TAggregate, TResult>(IEnumerable<TSource> source, Func<TSource, TAggregate, TAggregate> aggregate, TAggregate seed, Func<TAggregate, TResult> transform)
        {
            new { transform }.CheckNotNull();

            return transform(FoldRight(source, aggregate, seed));
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the end of the sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <typeparam name="TAggregate">The type of the aggregated value.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <param name="seed">A seed value for the aggregation.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TAggregate FoldRight<TSource, TAggregate>(IEnumerable<TSource> source, Func<TSource, TAggregate, TAggregate> aggregate, TAggregate seed)
        {
            new { source, aggregate }.CheckNotNull();

            TAggregate accumulator = seed;
            source = source.Reverse();
            foreach (TSource item in source)
            {
                accumulator = aggregate(item, accumulator);
            }

            return accumulator;
        }

        /// <summary>
        /// Applies a function over a sequence of items, starting from the end of the sequence and using the final list element as a seed.
        /// </summary>
        /// <typeparam name="TSource">The type of the items of the sequence.</typeparam>
        /// <param name="source">The sequence over which the function is to be applied.</param>
        /// <param name="aggregate">The function to be applied.</param>
        /// <returns>The aggregated value of the sequence under <paramref name="aggregate"/>.</returns>
        public static TSource FoldRight<TSource>(IEnumerable<TSource> source, Func<TSource, TSource, TSource> aggregate)
        {
            new { source, aggregate }.CheckNotNull();

            source = source.Reverse();
            TSource accumulator = source.First();
            source = source.Skip(1);
            foreach (TSource item in source)
            {
                accumulator = aggregate(item, accumulator);
            }

            return accumulator;
        }

        /// <summary>
        /// Generates a fixed point for a zero-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Func<TResult> Fix<TResult>(Func<Func<TResult>, Func<TResult>> f)
        {
            return () => f(Fix(f))();
        }

        /// <summary>
        /// Generates a fixed point for a one-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Func<T, TResult> Fix<T, TResult>(Func<Func<T, TResult>, Func<T, TResult>> f)
        {
            return a => f(Fix(f))(a);
        }

        /// <summary>
        /// Generates a fixed point for a two-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Func<T1, T2, TResult> Fix<T1, T2, TResult>(Func<Func<T1, T2, TResult>, Func<T1, T2, TResult>> f)
        {
            return (a, b) => f(Fix(f))(a, b);
        }

        /// <summary>
        /// Generates a fixed point for a three-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Func<T1, T2, T3, TResult> Fix<T1, T2, T3, TResult>(Func<Func<T1, T2, T3, TResult>, Func<T1, T2, T3, TResult>> f)
        {
            return (a, b, c) => f(Fix(f))(a, b, c);
        }

        /// <summary>
        /// Generates a fixed point for a four-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Func<T1, T2, T3, T4, TResult> Fix<T1, T2, T3, T4, TResult>(Func<Func<T1, T2, T3, T4, TResult>, Func<T1, T2, T3, T4, TResult>> f)
        {
            return (a, b, c, d) => f(Fix(f))(a, b, c, d);
        }

        /// <summary>
        /// Generates a fixed point for a zero-place function using the Y combinator.
        /// </summary>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Action FixAction(Func<Action, Action> f)
        {
            return () => f(FixAction(f))();
        }

        /// <summary>
        /// Generates a fixed point for a one-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Action<T> FixAction<T>(Func<Action<T>, Action<T>> f)
        {
            return a => f(FixAction(f))(a);
        }

        /// <summary>
        /// Generates a fixed point for a two-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Action<T1, T2> FixAction<T1, T2>(Func<Action<T1, T2>, Action<T1, T2>> f)
        {
            return (a, b) => f(FixAction(f))(a, b);
        }

        /// <summary>
        /// Generates a fixed point for a three-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Action<T1, T2, T3> FixAction<T1, T2, T3>(Func<Action<T1, T2, T3>, Action<T1, T2, T3>> f)
        {
            return (a, b, c) => f(FixAction(f))(a, b, c);
        }

        /// <summary>
        /// Generates a fixed point for a four-place function using the Y combinator.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function.</typeparam>
        /// <param name="f">The function whose fixed point is to be generated.</param>
        /// <returns>A fixed point for the specified function.</returns>
        public static Action<T1, T2, T3, T4> FixAction<T1, T2, T3, T4>(Func<Action<T1, T2, T3, T4>, Action<T1, T2, T3, T4>> f)
        {
            return (a, b, c, d) => f(FixAction(f))(a, b, c, d);
        }
    }
}